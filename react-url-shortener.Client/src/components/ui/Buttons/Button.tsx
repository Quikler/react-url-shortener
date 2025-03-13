import { twMerge } from "tailwind-merge";
import "./Buttons.css";
import useButtonVariant from "./useButtonVariant";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary" | "secondary" | "danger" | "info";
};

const Button = ({ variant = "primary", className, ...rest }: ButtonProps) => {
  const v = useButtonVariant(variant);
  return <button {...rest} className={twMerge(className, v)} />;
};

export default Button;
