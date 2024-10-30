import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import { RowDictionary } from '../components/RowDictionary';
import { REQUEST_URLS } from '../Constants';
import fetchMock from 'jest-fetch-mock';


describe('RowDictionary Component', () => {
  
  const initialProps = {
    value: 'InitialValue',
    language: 'en',
    keyName: 'InitialKey',
  };

  const renderRowDictionary = (props) => {
    return render(
        <table>
          <thead>
            <tr>
              <th>Key</th>
              <th>Value</th>
            </tr>
          </thead>
          <tbody>
            <RowDictionary {...props} />
          </tbody>
        </table>
    );
  };

  beforeEach(() => {
    global.fetch = require('jest-fetch-mock').default;
    fetchMock.enableMocks();
    fetchMock.doMock();
  });

  it('renders the component with initial props', () => {
    renderRowDictionary(initialProps);
    expect(screen.getByText('InitialValue')).toBeInTheDocument();
    expect(screen.getByText('InitialKey')).toBeInTheDocument();
  });

  it('opens modal for editing key when Edit button is clicked on key', async () => {
    renderRowDictionary(initialProps);
    fireEvent.click(screen.getAllByRole('button')[0]);
    const modal = await screen.getByText('Enter key value');
    expect(modal).toBeInTheDocument();
  });

  it('sends edit value request and updates state on confirm', async () => {
    
    var newValue = "newValue";
    
    fetch.mockResponseOnce(JSON.stringify(
        {
            rowId: 1,
            value: newValue
        }
      ));
    
    renderRowDictionary(initialProps);
    
    fireEvent.click(screen.getAllByRole('button')[1]);

    const modalInput = screen.getByRole('textbox');
    fireEvent.change(modalInput, {
      target: { value: newValue },
    });
    
    fireEvent.click( 
        screen.getByTestId('CheckIcon').parentElement
    );

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(REQUEST_URLS.EditValue, expect.anything());
      expect(screen.getByText(newValue)).toBeInTheDocument();
    });
  });

  
  it('sends edit key request and updates state on confirm', async () => {
    
    var newKey = "newKey";

    fetch.mockResponseOnce(JSON.stringify(
        {
            keyId: 1,
            keyValue: newKey
        }
      ));

    renderRowDictionary(initialProps);
    fireEvent.click(screen.getAllByRole('button')[0]);

    fireEvent.change(screen.getByDisplayValue(initialProps.keyName), {
      target: { value: newKey },
    });
    fireEvent.click(screen.getAllByRole('button')[0]);

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(REQUEST_URLS.EditKey, expect.anything());
      expect(screen.getByText(newKey)).toBeInTheDocument();
    });
  });
  

});