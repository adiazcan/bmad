import { msalInstance, loginRequest } from './auth';
import { useAuthStore } from '../stores/authStore';

// Get backend URL from environment variable (set by Aspire)
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

console.log('API Base URL:', API_BASE_URL);

interface ApiRequestOptions extends RequestInit {
  skipAuth?: boolean; // For public endpoints
}

/**
 * Authenticated fetch wrapper that automatically includes Bearer token
 * @param endpoint - API endpoint path (e.g., '/secure', '/conversations')
 * @param options - Fetch options with optional skipAuth flag
 */
export async function apiClient<T>(
  endpoint: string,
  options: ApiRequestOptions = {}
): Promise<T> {
  const { skipAuth = false, ...fetchOptions } = options;

  let headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...fetchOptions.headers,
  };

  // Add Bearer token if not skipping authentication
  if (!skipAuth) {
    const token = await getAccessToken();
    if (token) {
      headers = {
        ...headers,
        Authorization: `Bearer ${token}`,
      };
    } else {
      throw new Error('No access token available. User must sign in.');
    }
  }

  const url = `${API_BASE_URL}${endpoint}`;
  
  const response = await fetch(url, {
    ...fetchOptions,
    headers,
  });

  if (!response.ok) {
    if (response.status === 401) {
      // Token expired or invalid - attempt silent token refresh
      try {
        const newToken = await refreshAccessToken();
        if (newToken) {
          // Retry request with new token
          return apiClient<T>(endpoint, options);
        }
      } catch (refreshError) {
        console.error('Token refresh failed:', refreshError);
        // Clear auth state and redirect to login
        useAuthStore.getState().clearAuth();
        throw new Error('Authentication expired. Please sign in again.');
      }
    }

    // Handle transient errors with retry (500, 503, 429)
    if ([500, 503, 429].includes(response.status)) {
      const retryCount = (options as any).__retryCount || 0;
      const maxRetries = 3;
      
      if (retryCount < maxRetries) {
        // Exponential backoff: 1s, 2s, 4s
        const delayMs = Math.pow(2, retryCount) * 1000;
        console.warn(`API Error ${response.status}, retrying in ${delayMs}ms (attempt ${retryCount + 1}/${maxRetries})`);
        
        await new Promise(resolve => setTimeout(resolve, delayMs));
        
        // Retry with incremented counter
        return apiClient<T>(endpoint, { 
          ...options, 
          __retryCount: retryCount + 1 
        } as ApiRequestOptions);
      }
    }
    
    const errorBody = await response.text();
    throw new Error(`API Error ${response.status}: ${errorBody}`);
  }

  return response.json();
}

/**
 * Get access token from MSAL cache or acquire silently
 */
async function getAccessToken(): Promise<string | null> {
  const accounts = msalInstance.getAllAccounts();
  if (accounts.length === 0) {
    return null;
  }

  const request = {
    ...loginRequest,
    account: accounts[0],
  };

  try {
    const response = await msalInstance.acquireTokenSilent(request);
    useAuthStore.getState().setToken(response.accessToken);
    return response.accessToken;
  } catch (error) {
    console.error('Silent token acquisition failed:', error);
    return null;
  }
}

/**
 * Attempt to refresh access token silently
 */
async function refreshAccessToken(): Promise<string | null> {
  const accounts = msalInstance.getAllAccounts();
  if (accounts.length === 0) {
    return null;
  }

  const request = {
    ...loginRequest,
    account: accounts[0],
    forceRefresh: true, // Force refresh even if cached token exists
  };

  try {
    const response = await msalInstance.acquireTokenSilent(request);
    useAuthStore.getState().setToken(response.accessToken);
    return response.accessToken;
  } catch (error) {
    console.error('Token refresh failed:', error);
    return null;
  }
}

// Example usage:
// const data = await apiClient<WeatherForecast[]>('/weatherforecast', { skipAuth: true });
// const secureData = await apiClient<string>('/secure'); // Automatically includes Bearer token
