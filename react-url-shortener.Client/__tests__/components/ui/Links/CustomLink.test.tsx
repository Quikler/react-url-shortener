import { render, screen } from "@testing-library/react";
import CustomLink from "@src/components/ui/Links/CustomLink";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import userEvent from "@testing-library/user-event";

describe("CustomLink", () => {
  it("clicks and navigates to about page", async () => {
    render(
      <MemoryRouter initialEntries={["/"]}>
        <Routes>
          <Route path="/" element={<CustomLink to="/about">About</CustomLink>} />
          <Route path="/about" element={<div>About Page</div>} />
        </Routes>
      </MemoryRouter>
    );

    const linkElement = screen.getByRole("link", { name: /about/i });
    await userEvent.click(linkElement);
    expect(screen.getByText("About Page")).toBeInTheDocument();
  });
});
