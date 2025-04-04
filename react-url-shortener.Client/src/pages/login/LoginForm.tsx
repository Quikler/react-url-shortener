import Button from "@src/components/ui/Buttons/Button";
import Input from "@src/components/ui/Inputs/Input";
import Label from "@src/components/ui/Labels/Label";
import { Link } from "react-router-dom";
import { useForm } from "react-hook-form";
import ErrorMessage from "@src/components/ui/ErrorMessage";
import { useAuth } from "@src/hooks/useAuth";
import { handleErrorWithToast } from "@src/utils/helpers";
import { toast } from "@src/hooks/useToast";

const LoginForm = () => {
  const { loginUser } = useAuth();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm({
    mode: "onTouched",
    defaultValues: {
      username: "",
      password: "",
    },
  });

  const handleFormSubmit = handleSubmit(async (data) => {
    try {
      await loginUser(data);
      toast.success("Login successfully");
    } catch (e: any) {
      handleErrorWithToast(e);
    }
  });

  return (
    <form onSubmit={handleFormSubmit} className="mt-8 space-y-4">
      <div className="flex flex-col gap-2">
        <Label>Username</Label>
        <Input
          {...register("username", {
            required: "Username is required",
          })}
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
            minLength: { value: 8, message: "Password must be at least 8 characters long" },
          })}
          type="password"
          className="w-full"
          placeholder="Enter password"
        />
        <ErrorMessage>{errors.password?.message}</ErrorMessage>
      </div>
      <Button className="w-full">Sign in</Button>
      <p className="text-sm text-center">
        Don't have an account?
        <Link
          to="/signup"
          className="text-blue-600 hover:underline ml-1 whitespace-nowrap font-semibold"
        >
          Register here
        </Link>
      </p>
    </form>
  );
};

export default LoginForm;
