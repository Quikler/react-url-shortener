import Input from "@src/components/ui/Inputs/Input";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import React from "react";

describe("Input", () => {
  it("forwards ref correctly", () => {
    const ref = React.createRef<HTMLInputElement>();
    render(<Input ref={ref} placeholder="Enter text" />);

    const inputElement = screen.getByPlaceholderText("Enter text");
    expect(ref.current).toBe(inputElement); // Verify ref points to the input element
  });

  it("handles user input correctly", async () => {
    render(<Input placeholder="Enter text" />);

    const inputElement = screen.getByPlaceholderText("Enter text");
    await userEvent.type(inputElement, "Hello, World!");

    expect(inputElement).toHaveValue("Hello, World!"); // Verify input value
  });
});
