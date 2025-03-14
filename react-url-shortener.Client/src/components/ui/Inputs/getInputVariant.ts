import getVariant from "@src/utils/helpers";
import "./Inputs.css";

const useInputVariant = (variant: string) => {
  const v = getVariant(
    [
      {
        key: "primary",
        style: "input-primary",
      },
      {
        key: "secondary",
        style: "input-secondary",
      },
    ],
    variant,
    "input-default"
  );

  return v;
};

export default useInputVariant;
