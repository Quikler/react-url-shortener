import { useAuth } from "@src/hooks/useAuth";
import React from "react";

type AuthComponentProps = { children: React.ReactNode };

const AuthComponent = ({ children }: AuthComponentProps) => {
  const { isUserLoggedIn } = useAuth();
  return isUserLoggedIn() && children;
};

export default AuthComponent;
