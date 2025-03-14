import AuthComponent from "@src/components/HOC/AuthComponent";
import { useAuth } from "@src/hooks/useAuth";
import { render, screen } from "@testing-library/react";

jest.mock("@src/hooks/useAuth", () => ({
  useAuth: jest.fn(),
}));

describe("AuthComponent", () => {
  it("renders children when user is logged in", () => {
    (useAuth as jest.Mock).mockReturnValue({
      isUserLoggedIn: () => true,
    });

    render(
      <AuthComponent>
        <div>Auth content</div>
      </AuthComponent>
    );

    const authContent = screen.getByText("Auth content");
    expect(authContent).toBeInTheDocument();
  });

	it("does not render children when user is not authenticated", () => {
    (useAuth as jest.Mock).mockReturnValue({
      isUserLoggedIn: () => false,
    });

    render(
      <AuthComponent>
        <div>Auth content</div>
      </AuthComponent>
    );

    const authContent = screen.queryByText("Auth content");
    expect(authContent).not.toBeInTheDocument();
  });
});
