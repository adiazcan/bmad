// Get backend URL from environment variable (set by Aspire)
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

console.log('API Base URL:', API_BASE_URL);

/**
 * Fetch wrapper that prepends API base URL to all requests
 */
export async function apiClient<T>(
  endpoint: string,
  options?: RequestInit
): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;
  
  const response = await fetch(url, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  });

  if (!response.ok) {
    throw new Error(`API Error: ${response.status} ${response.statusText}`);
  }

  return response.json();
}

// Example usage:
// const data = await apiClient<WeatherForecast[]>('/weatherforecast');
