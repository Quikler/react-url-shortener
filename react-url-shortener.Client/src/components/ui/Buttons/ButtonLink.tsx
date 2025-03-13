import { Link, LinkProps } from "react-router-dom";
import { twMerge } from "tailwind-merge";
import "./Buttons.css";
import useButtonVariant from "./useButtonVariant";

type ButtonLinkProps = LinkProps & {
  variant?: "primary" | "secondary" | "info";
};

const ButtonLink = ({ variant = "primary", className, ...rest }: ButtonLinkProps) => {
  const v = useButtonVariant(variant);
  return <Link {...rest} className={twMerge(className, v)} />;
};

export default ButtonLink;
