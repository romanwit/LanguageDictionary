import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import { RowDictionary } from '../components/RowDictionary';
import { ModalInput } from '../components/ModalInput';
import { REQUEST_URLS } from '../Constants';

// Mock fetch API
global.fetch = jest.fn(() =>
  Promise.resolve({
    json: () => Promise.resolve({ value: 'newValue', keyValue: 'newKey' }),
    status: 200,
  })
);

describe('RowDictionary Component', () => {
  const initialProps = {
    value: 'InitialValue',
    language: 'en',
    keyName: 'InitialKey',
  };

  beforeEach(() => {
    fetch.mockClear();
  });

  it('renders the component with initial props', () => {
    render(<RowDictionary {...initialProps} />);
    expect(screen.getByText('InitialValue')).toBeInTheDocument();
    expect(screen.getByText('InitialKey')).toBeInTheDocument();
  });

  it('opens modal for editing value when Edit button is clicked', () => {
    render(<RowDictionary {...initialProps} />);
    fireEvent.click(screen.getAllByRole('button')[1]);
    expect(screen.getByText('Enter value')).toBeInTheDocument();
  });

  it('opens modal for editing key when Edit button is clicked on key', () => {
    render(<RowDictionary {...initialProps} />);
    fireEvent.click(screen.getAllByRole('button')[0]);
    expect(screen.getByText('Enter key value')).toBeInTheDocument();
  });

  it('sends edit value request and updates state on confirm', async () => {
    render(<RowDictionary {...initialProps} />);
    fireEvent.click(screen.getAllByRole('button')[1]);

    // Simulate modal input confirm
    const modalInput = screen.getByText('Enter value');
    fireEvent.change(screen.getByDisplayValue(initialProps.value), {
      target: { value: 'newValue' },
    });
    fireEvent.click(screen.getByText('Confirm'));

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(REQUEST_URLS.EditValue, expect.anything());
      expect(screen.getByText('newValue')).toBeInTheDocument();
    });
  });

  it('sends edit key request and updates state on confirm', async () => {
    render(<RowDictionary {...initialProps} />);
    fireEvent.click(screen.getAllByRole('button')[0]);

    // Simulate modal input confirm
    fireEvent.change(screen.getByDisplayValue(initialProps.keyName), {
      target: { value: 'newKey' },
    });
    fireEvent.click(screen.getByText('Confirm'));

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(REQUEST_URLS.EditKey, expect.anything());
      expect(screen.getByText('newKey')).toBeInTheDocument();
    });
  });

  it('cancels editing and reverts to initial state on cancel', () => {
    render(<RowDictionary {...initialProps} />);
    fireEvent.click(screen.getAllByRole('button')[1]);

    // Simulate modal input cancel
    fireEvent.click(screen.getByText('Cancel'));
    expect(screen.getByText('InitialValue')).toBeInTheDocument();
  });
});
