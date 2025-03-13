import Label from "@src/components/ui/Labels/Label";
import { render, screen } from "@testing-library/react";

describe("Label", () => {
  it("renders children correctly", () => {
    render(
      <Label>
        <span>Child Element</span>
      </Label>
    );

    const childElement = screen.getByText("Child Element");
    expect(childElement).toBeInTheDocument();
  });
});
