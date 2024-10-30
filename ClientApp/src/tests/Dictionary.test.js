import React from 'react';
import { render, screen, fireEvent, waitFor, act } from '@testing-library/react';
import '@testing-library/jest-dom';
import fetchMock from 'jest-fetch-mock';
import { Dictionary } from '../components/Dictionary';
import { REQUEST_URLS } from '../Constants';

global.fetch = require('jest-fetch-mock').default;

describe('<Dictionary />', () => {
    beforeEach(() => {
        
        fetchMock.enableMocks();
        fetchMock.doMock();

        fetch.mockResponseOnce(JSON.stringify([
            { 
                key: { 
                    keyId: 1,
                    keyValue: 'testKey' 
                }, 
                value: 'testValue' 
            }
        ]));

      });

  test('renders loading message initially', () => {

    act(()=>{
        render(<Dictionary language="en" />);
    });
    expect(screen.getByText(/Loading.../i)).toBeInTheDocument();
  });

  test('calls populateDetails on mount and renders table', async () => {
    act(()=>{
        render(<Dictionary language="en" />);
    });
    expect(fetch).toHaveBeenCalledWith(`${REQUEST_URLS.Details}=en`);
    await waitFor(() => expect(screen.getByText('testKey')).toBeInTheDocument());
    expect(screen.getByText('testValue')).toBeInTheDocument();
  });



});
