import { useAuth } from "@src/hooks/useAuth";

type AdminComponentProps = {
  children: React.ReactNode;
};

const AdminComponent = ({ children }: AdminComponentProps) => {
  const { hasRole } = useAuth();
  return hasRole("Admin") && children;
};

export default AdminComponent;
