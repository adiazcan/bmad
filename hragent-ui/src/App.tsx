import { useEffect, useState } from 'react';
import { apiClient } from './lib/api-client';
import reactLogo from './assets/react.svg';
import viteLogo from '/vite.svg';
import './App.css';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string | null;
}

function App() {
  const [weather, setWeather] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    apiClient<WeatherForecast[]>('/weatherforecast')
      .then(data => {
        setWeather(data);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

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
      <h1>HRAgent - Weather Test</h1>
      
      <div className="card">
        <h2>Backend API Connection Test</h2>
        {loading && <p>Loading weather data from backend...</p>}
        {error && <p style={{ color: 'red' }}>Error: {error}</p>}
        {!loading && !error && weather.length > 0 && (
          <div style={{ textAlign: 'left' }}>
            <p><strong>✅ Successfully connected to backend API!</strong></p>
            <h3>Weather Forecast:</h3>
            <ul>
              {weather.map((item, index) => (
                <li key={index}>
                  {item.date}: {item.temperatureC}°C ({item.temperatureF}°F) - {item.summary}
                </li>
              ))}
            </ul>
          </div>
        )}
      </div>
      
      <p className="read-the-docs">
        This page verifies that .NET Aspire orchestration is working correctly
      </p>
    </>
  );
}

export default App;
