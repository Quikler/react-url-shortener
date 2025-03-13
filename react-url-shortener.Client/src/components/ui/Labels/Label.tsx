import useVariant from "@src/hooks/useVariant";
import { twMerge } from "tailwind-merge";
import "./Labels.css";

type LabelProps = React.LabelHTMLAttributes<HTMLLabelElement> & {
  variant?: "primary";
};

const Label = ({ variant = "primary", className, ...rest }: LabelProps) => {
  const v = useVariant([], variant, "label-default");

  return <label className={twMerge(className, v)} {...rest} />;
};

export default Label;
