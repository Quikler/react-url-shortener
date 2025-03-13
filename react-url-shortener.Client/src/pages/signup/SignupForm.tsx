import Button from "@src/components/ui/Buttons/Button";
import ErrorMessage from "@src/components/ui/ErrorMessage";
import Input from "@src/components/ui/Inputs/Input";
import Label from "@src/components/ui/Labels/Label";
import { useAuth } from "@src/hooks/useAuth";
import { useToast } from "@src/hooks/useToast";
import { handleError } from "@src/utils/helpers";
import { useForm } from "react-hook-form";
import { Link } from "react-router-dom";

const SignupForm = () => {
  const { signupUser } = useAuth();
  const { danger, success } = useToast();

  const {
    register,
    trigger,
    handleSubmit,
    formState: { errors },
  } = useForm({
    mode: "onTouched",
    defaultValues: {
      username: "",
      password: "",
      confirmPassword: "",
    },
  });

  const handleFormSubmit = handleSubmit(async (data) => {
    try {
      await signupUser(data);
      success("Signup successfully");
    } catch (e: any) {
      handleError(e, danger);
    }
  });

  return (
    <form className="mt-8 space-y-4" onSubmit={handleFormSubmit}>
      <div className="flex flex-col gap-2">
        <Label>Username</Label>
        <Input
          {...register("username", { required: "Username is required" })}
          type="text"
          className="w-full"
          placeholder="Enter username"
        />
        <ErrorMessage>{errors.username?.message}</ErrorMessage>
      </div>
      <div className="flex flex-col gap-2">
        <Label>Password</Label>
        <Input
          {...register("password", {
            required: "Password is required",
            validate: (password, fields) => {
              if (password === fields.confirmPassword) {
                trigger("confirmPassword");
              } else {
                return "Passwords do not match";
              }
            },
            minLength: { value: 8, message: "Password must be at least 8 characters long" },
          })}
          type="password"
          className="w-full"
          placeholder="Enter password"
        />
        <ErrorMessage>{errors.password?.message}</ErrorMessage>
      </div>
      <div className="flex flex-col gap-2">
        <Label>Confirm Password</Label>
        <Input
          {...register("confirmPassword", {
            required: "Password is required",
            validate: (confirmPassword, fields) => {
              if (confirmPassword === fields.password) {
                trigger("password");
              } else {
                return "Passwords do not match";
              }
            },
          })}
          type="password"
          className="w-full"
          placeholder="Enter password"
        />
        <ErrorMessage>{errors.confirmPassword?.message}</ErrorMessage>
      </div>
      <Button className="w-full">Sign up</Button>
      <p className="text-gray-800 text-sm text-center">
        Already have an account?
        <Link
          to="/login"
          className="text-blue-600 hover:underline ml-1 whitespace-nowrap font-semibold"
        >
          Sign in here
        </Link>
      </p>
    </form>
  );
};

export default SignupForm;
