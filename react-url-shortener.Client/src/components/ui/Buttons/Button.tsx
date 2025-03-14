import { twMerge } from "tailwind-merge";
import "./Buttons.css";
import getButtonVariant from "./useButtonVariant";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary" | "secondary" | "danger" | "info";
};

const Button = ({ variant = "primary", className, ...rest }: ButtonProps) => {
  const v = getButtonVariant(variant);
  return <button {...rest} className={twMerge(className, v)} />;
};

export default Button;
