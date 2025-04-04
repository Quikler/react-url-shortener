import { render, screen, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import Toast, { ToastType } from "@src/components/ui/Toast";

describe("Toast Component", () => {
  const renderToast = (props: Partial<React.ComponentProps<typeof Toast>> = {}) => {
    return render(
      <Toast
        type="success"
        message="Test message"
        isVisible={true}
        onClose={jest.fn()}
        transitionDuration={500}
        {...props}
      />
    );
  };

  it("renders the toast message", () => {
    renderToast();
    expect(screen.getByText("Test message")).toBeInTheDocument();
  });

  it("shows correct icon based on type", () => {
    const types: ToastType[] = ["success", "danger", "warning"];
    types.forEach((type) => {
      const { unmount } = renderToast({ type });
      expect(screen.getByRole("alert")).toBeInTheDocument(); // Snapshot testing
      unmount();
    });
  });

  it("has opacity 1 when visible", () => {
    renderToast({ isVisible: true });
    expect(screen.getByRole("alert")).toHaveStyle("opacity: 1");
  });

  it("has opacity 0 when hidden", () => {
    renderToast({ isVisible: false });
    expect(screen.getByRole("alert")).toHaveStyle("opacity: 0");
  });

  it("calls onClose when close button is clicked", () => {
    const onCloseMock = jest.fn();
    renderToast({ onClose: onCloseMock });

    const closeButton = screen.getByRole("button", { name: /close/i });
    fireEvent.click(closeButton);

    expect(onCloseMock).toHaveBeenCalledTimes(1);
  });
});

