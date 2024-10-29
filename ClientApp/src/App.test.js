import React from 'react';
import { createRoot } from 'react-dom/client';
import { MemoryRouter } from 'react-router-dom';
import App from './App';
import fetchMock from 'jest-fetch-mock';
import { REQUEST_URLS } from './Constants'

beforeEach(() => {
  global.fetch = require('jest-fetch-mock').default;
  fetchMock.enableMocks();
  fetchMock.doMock();
});


it('renders without crashing', async () => {
  fetch.mockResponseOnce(JSON.stringify(['English', 'Francais']));
  fetch.mockResponseOnce(JSON.stringify(
    [
      {
        rowId: 1, 
        key: {
          keyId: 1,
          keyValue: 'Key'
        },
        language: {
          languageId: 1,
          languageValue: 'English'
        },
        value: "The Value"
      }
    ]
  ));
  const div = document.createElement('div');
  const root = createRoot(div);
  root.render(
    <MemoryRouter>
      <App />
    </MemoryRouter>);
  await new Promise(resolve => setTimeout(resolve, 1000));
});
