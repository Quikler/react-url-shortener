import Button from "@src/components/ui/Buttons/Button";
import { render, screen } from "@testing-library/react";

describe("Button", () => {
  it("Button displays with the correct text", () => {
    render(<Button>Test</Button>);
    const buttonElement = screen.getByRole("button", { name: "Test" });
    expect(buttonElement).toBeInTheDocument();
  });
});
