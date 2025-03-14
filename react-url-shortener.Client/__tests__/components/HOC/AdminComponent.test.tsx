import AdminComponent from "@src/components/HOC/AdminComponent";
import { useAuth } from "@src/hooks/useAuth";
import { render, screen } from "@testing-library/react";

jest.mock("@src/hooks/useAuth", () => ({
  useAuth: jest.fn(),
}));

describe("AdminComponent", () => {
  // Test 1: Renders children when the user has the "Admin" role
  it("renders children when the user has the 'Admin' role", () => {
    // Mock useAuth to return hasRole("Admin") => true
    (useAuth as jest.Mock).mockReturnValue({
      hasRole: (role: string) => role === "Admin",
    });

    render(
      <AdminComponent>
        <div>Admin Content</div>
      </AdminComponent>
    );

    const adminContent = screen.getByText("Admin Content");
    expect(adminContent).toBeInTheDocument();
  });

  // Test 2: Does not render children when the user does not have the "Admin" role
  it("does not render children when the user does not have the 'Admin' role", () => {
    // Mock useAuth to return hasRole("Admin") => false
    (useAuth as jest.Mock).mockReturnValue({
      hasRole: (role: string) => role !== "Admin",
    });

    render(
      <AdminComponent>
        <div>Admin Content</div>
      </AdminComponent>
    );

    const adminContent = screen.queryByText("Admin Content");
    expect(adminContent).not.toBeInTheDocument();
  });

  // Test 3: Does not render children when hasRole returns false for any role
  it("does not render children when hasRole returns false for any role", () => {
    // Mock useAuth to return hasRole(role) => false for any role
    (useAuth as jest.Mock).mockReturnValue({
      hasRole: () => false,
    });

    render(
      <AdminComponent>
        <div>Admin Content</div>
      </AdminComponent>
    );

    const adminContent = screen.queryByText("Admin Content");
    expect(adminContent).not.toBeInTheDocument();
  });
});
