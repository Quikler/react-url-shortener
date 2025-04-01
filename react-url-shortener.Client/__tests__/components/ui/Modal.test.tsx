import Modal from "@src/components/ui/Modal"
import { fireEvent, render, screen } from "@testing-library/react"

describe('Modal', () => {
  const defaultProps = {
    bgClass: "bg-black/50",
    title: "Test Modal",
    isOpen: true,
    onClose: jest.fn(),
    children: <p>Modal Content</p>,
  };

  it("renders the modal when isOpen is true", () => {
    render(<Modal {...defaultProps} />);

    expect(screen.getByText("Test Modal")).toBeInTheDocument();
    expect(screen.getByText("Modal Content")).toBeInTheDocument();
  });

  it("doesn't renders children when isOpen is false", () => {
    render(<Modal {...defaultProps} isOpen={false} />)

    expect(screen.queryByText("Test Modal")).not.toBeInTheDocument();
    expect(screen.queryByText("Modal Content")).not.toBeInTheDocument();
  })

  it("calls onClose when close button was clicked", () => {
    const onCloseMock = jest.fn();
    render(<Modal {...defaultProps} onClose={onCloseMock} />)

    const button = screen.getByRole("button");
    fireEvent.click(button);

    expect(onCloseMock).toHaveBeenCalledTimes(1);
  })
})
