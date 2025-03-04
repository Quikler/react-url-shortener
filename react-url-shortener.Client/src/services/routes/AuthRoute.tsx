import { useAuth } from "@src/hooks/useAuth";
import React from "react";
import { Navigate } from "react-router-dom";

type AuthRouteProps = { children: React.ReactNode };

const AuthRoute = ({ children }: AuthRouteProps) => {
  const { isUserLoggedIn } = useAuth();
  return isUserLoggedIn() ? children : <Navigate to="/login" />;
};

export default AuthRoute;
