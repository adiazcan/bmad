import { MsalProvider, useMsal, useIsAuthenticated } from '@azure/msal-react';
import { useEffect, useState } from 'react';
import { msalInstance, initializeMsal, loginRequest } from './lib/auth';
import { useAuthStore } from './stores/authStore';
import { apiClient } from './lib/api-client';
import reactLogo from './assets/react.svg';
import viteLogo from '/vite.svg';
import './App.css';

function App() {
  const [initialized, setInitialized] = useState(false);

  useEffect(() => {
    initializeMsal().then(() => setInitialized(true));
  }, []);

  if (!initialized) {
    return <div>Loading authentication...</div>;
  }

  return (
    <MsalProvider instance={msalInstance}>
      <AuthenticatedApp />
    </MsalProvider>
  );
}

function AuthenticatedApp() {
  const { instance, accounts } = useMsal();
  const isAuthenticated = useIsAuthenticated();
  const { setUser, clearAuth } = useAuthStore();

  useEffect(() => {
    if (accounts.length > 0) {
      setUser(accounts[0]);
      // Token will be acquired on first API call via api-client.ts
    }
  }, [accounts, setUser]);

  const handleLogin = async () => {
    try {
      await instance.loginRedirect(loginRequest);
    } catch (error) {
      console.error('Login failed:', error);
    }
  };

  const handleLogout = async () => {
    try {
      clearAuth();
      await instance.logoutRedirect({
        account: accounts[0],
      });
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  const testSecureEndpoint = async () => {
    try {
      const result = await apiClient<{ message: string; timestamp: string }>('/secure');
      alert(`Success: ${result.message}\nTimestamp: ${result.timestamp}`);
    } catch (error) {
      alert(`Error: ${error instanceof Error ? error.message : 'Unknown error'}`);
    }
  };

  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>HRAgent</h1>
      
      <div className="card">
        {isAuthenticated ? (
          <div>
            <p><strong>Welcome, {accounts[0]?.name}!</strong></p>
            <p style={{ fontSize: '0.9em', color: '#888' }}>
              {accounts[0]?.username}
            </p>
            <div style={{ display: 'flex', gap: '10px', justifyContent: 'center', marginTop: '1em' }}>
              <button onClick={testSecureEndpoint}>Test Secure API</button>
              <button onClick={handleLogout}>Sign Out</button>
            </div>
          </div>
        ) : (
          <div>
            <p>Please sign in to access HRAgent</p>
            <button onClick={handleLogin}>Sign In with Azure AD</button>
          </div>
        )}
      </div>
      
      <p className="read-the-docs">
        Azure AD authentication with MSAL.js
      </p>
    </>
  );
}

export default App;
