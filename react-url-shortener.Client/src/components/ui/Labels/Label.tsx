import { twMerge } from "tailwind-merge";
import "./Labels.css";
import getVariant from "@src/utils/helpers";

type LabelProps = React.LabelHTMLAttributes<HTMLLabelElement> & {
  variant?: "primary";
};

const Label = ({ variant = "primary", className, ...rest }: LabelProps) => {
  const v = getVariant([], variant, "label-default");
  return <label className={twMerge(className, v)} {...rest} />;
};

export default Label;
