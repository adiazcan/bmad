import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import App from '../App';

describe('App Component', () => {
  it('renders without crashing', () => {
    const { container } = render(<App />);
    expect(container.firstChild).toBeTruthy();
  });

  it('renders Vite logo', () => {
    render(<App />);
    const viteLogo = screen.getByAltText('Vite logo');
    expect(viteLogo).toBeInTheDocument();
  });

  it('renders React logo', () => {
    render(<App />);
    const reactLogo = screen.getByAltText('React logo');
    expect(reactLogo).toBeInTheDocument();
  });

  it('displays main heading', () => {
    render(<App />);
    const heading = screen.getByRole('heading', { level: 1 });
    expect(heading).toBeInTheDocument();
    expect(heading.textContent).toContain('Vite');
    expect(heading.textContent).toContain('React');
  });

  it('has a clickable button', () => {
    render(<App />);
    const button = screen.getByRole('button');
    expect(button).toBeInTheDocument();
    expect(button.textContent).toContain('count is');
  });

  it('button click updates count', async () => {
    const user = userEvent.setup();
    render(<App />);
    const button = screen.getByRole('button');
    
    // Initial state
    expect(button.textContent).toContain('count is 0');
    
    // Click button
    await user.click(button);
    expect(button.textContent).toContain('count is 1');
    
    // Click again
    await user.click(button);
    expect(button.textContent).toContain('count is 2');
  });

  it('displays "Edit src/App.tsx" message', () => {
    render(<App />);
    const editMessage = screen.getByText(/and save to test HMR/);
    expect(editMessage).toBeInTheDocument();
    // Verify the code element contains the path
    const codeElement = screen.getByText('src/App.tsx');
    expect(codeElement).toBeInTheDocument();
  });

  it('displays "Click on the Vite and React logos" message', () => {
    render(<App />);
    const clickMessage = screen.getByText(/Click on the Vite and React logos to learn more/);
    expect(clickMessage).toBeInTheDocument();
  });
});
