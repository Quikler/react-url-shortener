import Button from "@src/components/ui/Buttons/Button";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";

describe("Button", () => {
  it("displays with the correct text", () => {
    render(<Button>Test</Button>);
    const buttonElement = screen.getByRole("button", { name: "Test" });
    expect(buttonElement).toBeInTheDocument();
  });

  it("displays with the correct default variant (primary)", () => {
    render(<Button>Test</Button>);
    const buttonElement = screen.getByRole("button", { name: "Test" });
    expect(buttonElement).toHaveClass("btn-primary");
  });

  it("displays with the correct secondary variant", () => {
    render(<Button variant="secondary">Secondary</Button>);
    const buttonElement = screen.getByRole("button", { name: "Secondary" });
    expect(buttonElement).toHaveClass("btn-secondary");
  });

  it("displays with the correct danger variant", () => {
    render(<Button variant="danger">Danger</Button>);
    const buttonElement = screen.getByRole("button", { name: "Danger" });
    expect(buttonElement).toHaveClass("btn-danger");
  });

  it("displays with the correct info variant", () => {
    render(<Button variant="info">Info</Button>);
    const buttonElement = screen.getByRole("button", { name: "Info" });
    expect(buttonElement).toHaveClass("btn-info");
  });

  it("calls onClick handler", async () => {
    const user = userEvent.setup();
    const onClick = jest.fn();
    render(<Button onClick={onClick}>Click</Button>);

    await user.click(screen.getByRole("button", { name: /click/i }));
    expect(onClick).toHaveBeenCalledTimes(1);
  });

  it("renders as disabled when disabled prop is set", () => {
    render(<Button disabled>Disabled</Button>);
    const button = screen.getByRole("button", { name: /disabled/i });
    expect(button).toBeDisabled();
  });
});
