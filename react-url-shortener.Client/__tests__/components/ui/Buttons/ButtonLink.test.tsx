import { render, screen } from "@testing-library/react";
import ButtonLink from "@src/components/ui/Buttons/ButtonLink";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import userEvent from "@testing-library/user-event";

describe("ButtonLink", () => {
  it("renders with default primary variant", () => {
    render(
      <MemoryRouter>
        <ButtonLink to="/">Click Me</ButtonLink>
      </MemoryRouter>
    );

    const linkElement = screen.getByRole("link", { name: /click me/i });
    expect(linkElement).toBeInTheDocument();
    expect(linkElement).toHaveClass("btn-primary");
  });

  it("renders with secondary variant", () => {
    render(
      <MemoryRouter>
        <ButtonLink to="/" variant="secondary">
          Click Me
        </ButtonLink>
      </MemoryRouter>
    );

    const linkElement = screen.getByRole("link", { name: /click me/i });
    expect(linkElement).toHaveClass("btn-secondary");
  });

  it("merges custom className with variant class", () => {
    render(
      <MemoryRouter>
        <ButtonLink to="/" className="custom-class" variant="info">
          Click Me
        </ButtonLink>
      </MemoryRouter>
    );

    const linkElement = screen.getByRole("link", { name: /click me/i });
    expect(linkElement).toHaveClass("btn-info");
    expect(linkElement).toHaveClass("custom-class");
  });

  it("clicks and navigates to about page", async () => {
    render(
      <MemoryRouter initialEntries={["/"]}>
        <Routes>
          <Route path="/" element={<ButtonLink to="/about">About</ButtonLink>} />
          <Route path="/about" element={<div>About Page</div>} />
        </Routes>
      </MemoryRouter>
    );

    const linkElement = screen.getByRole("link", { name: /about/i });
    await userEvent.click(linkElement);
    expect(screen.getByText("About Page")).toBeInTheDocument();
  });
});
