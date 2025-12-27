import { PublicClientApplication, LogLevel, type Configuration } from '@azure/msal-browser';

// Validate required environment variables
const requiredEnvVars = {
  VITE_AZURE_AD_CLIENT_ID: import.meta.env.VITE_AZURE_AD_CLIENT_ID,
  VITE_AZURE_AD_AUTHORITY: import.meta.env.VITE_AZURE_AD_AUTHORITY,
  VITE_AZURE_AD_REDIRECT_URI: import.meta.env.VITE_AZURE_AD_REDIRECT_URI,
  VITE_AZURE_AD_API_SCOPE: import.meta.env.VITE_AZURE_AD_API_SCOPE,
};

for (const [key, value] of Object.entries(requiredEnvVars)) {
  if (!value) {
    throw new Error(
      `Missing required environment variable: ${key}. ` +
      `Please check your .env.local file and ensure all Azure AD configuration values are set. ` +
      `See .env.example for required variables.`
    );
  }
}

const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_AD_CLIENT_ID,
    authority: import.meta.env.VITE_AZURE_AD_AUTHORITY,
    redirectUri: import.meta.env.VITE_AZURE_AD_REDIRECT_URI,
  },
  cache: {
    cacheLocation: 'localStorage', // Cross-tab token sharing
    storeAuthStateInCookie: false, // Set to true for IE11 or Edge legacy
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) return; // Never log PII
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            break;
          case LogLevel.Info:
            console.info(message);
            break;
          case LogLevel.Verbose:
            console.debug(message);
            break;
          case LogLevel.Warning:
            console.warn(message);
            break;
        }
      },
      logLevel: LogLevel.Info,
    },
  },
};

export const msalInstance = new PublicClientApplication(msalConfig);

// Initialize MSAL - must be called before using msalInstance
export const initializeMsal = async () => {
  await msalInstance.initialize();
  await msalInstance.handleRedirectPromise();
};

// Login request configuration
export const loginRequest = {
  scopes: [import.meta.env.VITE_AZURE_AD_API_SCOPE],
};
