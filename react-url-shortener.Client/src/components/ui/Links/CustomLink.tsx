import { Link, LinkProps, NavLink } from "react-router-dom";
import { twMerge } from "tailwind-merge";
import getLinkVariant from "./getLinkVariant";

type CustomLinkProps = LinkProps & {
  variant?: "primary" | "secondary";
};

const CustomLink = ({ variant = "primary", to = "#", className, ...rest }: CustomLinkProps) => {
  const v = getLinkVariant(variant);
  return to ? <Link {...rest} to={to} className={twMerge(className, v)} /> : <NavLink className={twMerge(className, v)} to="#" />;
};

export default CustomLink;
