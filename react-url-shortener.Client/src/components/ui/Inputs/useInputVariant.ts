import useVariant from "@src/hooks/useVariant";
import "./Inputs.css";

const useInputVariant = (variant: string) => {
  const v = useVariant(
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
