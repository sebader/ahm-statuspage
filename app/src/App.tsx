import React, { useState, useEffect } from 'react';
import StatusTable from './components/StatusTable';
import { ComponentStatus } from './types/ComponentStatus';
import { AppConfig } from './types/AppConfig';
import config from './config/app-config.json';
import './App.css';

function App() {
  const appConfig = config as AppConfig;
  const [components, setComponents] = useState<ComponentStatus[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchStatuses();
  }, []);

  const fetchStatuses = async () => {
    try {
      const response = await fetch('/api/GetStatus');
      if (!response.ok) {
        throw new Error('Failed to fetch status data');
      }
      const data = await response.json();
      setComponents(data);
      setError(null);
    } catch (err) {
      setError('Unable to load status information. Please try again later.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="App">
      <style>
        {`
          body {
            margin: 0;
            padding: 0;
            background-color: ${appConfig.theme.backgroundColor};
            font-family: ${appConfig.theme.fontFamily};
          }
          .App {
            padding: 2rem;
            max-width: 1200px;
            margin: 0 auto;
          }
          .header {
            margin-bottom: 2rem;
            text-align: center;
          }
          .footer {
            text-align: center;
            margin-top: 3rem;
            padding-top: 1rem;
            border-top: 1px solid #eee;
          }
          .company-name {
            color: #666;
            font-size: 0.9rem;
          }
          .header h1 {
            color: ${appConfig.theme.primaryColor};
            font-size: 2.5rem;
            margin-bottom: 0.25rem;
          }
          .header h2 {
            color: ${appConfig.theme.primaryColor};
            font-size: 2rem;
            margin: 1rem 0 0.5rem;
          }
          .header p {
            color: #666;
            font-size: 1.1rem;
            margin: 0;
          }
          @keyframes shimmer {
            0% {
              opacity: 1;
            }
            50% {
              opacity: 0.7;
            }
            100% {
              opacity: 1;
            }
          }

          .table-loading {
            animation: shimmer 1.5s infinite ease-in-out;
            pointer-events: none;
          }
          .error {
            background-color: #fff3f3;
            color: #e74c3c;
            padding: 1rem;
            border-radius: 8px;
            text-align: center;
            margin: 1rem 0;
          }
          .refresh-button {
            background: none;
            border: none;
            cursor: pointer;
            padding: 8px;
            border-radius: 4px;
            color: ${appConfig.theme.primaryColor};
            font-size: 1.2rem;
            transition: background-color 0.2s;
            display: flex;
            align-items: center;
            margin-left: auto;
          }
          .refresh-button:hover {
            background-color: rgba(0, 0, 0, 0.05);
          }
          .status-section {
            position: relative;
          }
          .status-header {
            display: flex;
            align-items: center;
            margin-bottom: 1rem;
          }
        `}
      </style>
      <div className="header">
        <h1>{appConfig.appName}</h1>
        <h2>{appConfig.pageHeader}</h2>
        <p>{appConfig.pageSubheader}</p>
      </div>
      {error ? (
        <div className="error">{error}</div>
      ) : (
        <div className={`status-section ${loading ? 'table-loading' : ''}`}>
          <div className="status-header">
            <button 
              className="refresh-button" 
              onClick={() => {
                setLoading(true);
                fetchStatuses();
              }}
              aria-label="Refresh status"
            >
              ↻
            </button>
          </div>
          <StatusTable components={components} />
        </div>
      )}
      <div className="footer">
        <div className="company-name">© {new Date().getFullYear()} {appConfig.companyName}</div>
      </div>
    </div>
  );
}

export default App;
