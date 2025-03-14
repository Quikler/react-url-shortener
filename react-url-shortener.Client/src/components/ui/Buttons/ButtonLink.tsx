import { Link, LinkProps } from "react-router-dom";
import { twMerge } from "tailwind-merge";
import "./Buttons.css";
import getButtonVariant from "./useButtonVariant";

type ButtonLinkProps = LinkProps & {
  variant?: "primary" | "secondary" | "info";
};

const ButtonLink = ({ variant = "primary", className, ...rest }: ButtonLinkProps) => {
  const v = getButtonVariant(variant);
  return <Link {...rest} className={twMerge(className, v)} />;
};

export default ButtonLink;
