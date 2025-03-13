import { render, screen } from "@testing-library/react";
import ErrorMessage from "@src/components/ui/ErrorMessage";

describe("ErrorMessage Component", () => {
  // Test 1: Renders a <p> element with the correct className
  it("renders a <p> element with the correct className", () => {
    render(<ErrorMessage />);

    const paragraphElement = screen.getByRole("paragraph");
    expect(paragraphElement).toBeInTheDocument();
    expect(paragraphElement).toHaveClass("text-red-500");
  });

  // Test 2: Passes down additional attributes correctly
  it("passes down additional attributes correctly", () => {
    render(
      <ErrorMessage id="error-message" aria-label="Error">
        Test Error
      </ErrorMessage>
    );

    const paragraphElement = screen.getByText("Test Error");
    expect(paragraphElement).toHaveAttribute("id", "error-message");
    expect(paragraphElement).toHaveAttribute("aria-label", "Error");
  });

  // Test 3: Renders children correctly
  it("renders children correctly", () => {
    render(<ErrorMessage>Test Error Message</ErrorMessage>);

    const paragraphElement = screen.getByText("Test Error Message");
    expect(paragraphElement).toBeInTheDocument();
  });
});
