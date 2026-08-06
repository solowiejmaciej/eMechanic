import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate, Link as RouterLink } from "react-router-dom";
import { VStack, Heading, Text, Stack, Input, Button, HStack, Icon, Field, InputGroup, Link } from "@chakra-ui/react";
import { PasswordInput } from "@/components/ui/password-input";
import { Mail, Lock, ArrowRight, User } from "lucide-react";
import { registerUser } from "../../api/auth";
import { toaster } from "@/components/ui/toaster";

const formatErrorMsg = (err, fallback) => {
  if (err.response?.data?.validationErrors) {
    const errorDetails = Object.values(err.response.data.validationErrors).flat().join(", ");
    if (errorDetails) return errorDetails;
  }
  if (err.response?.data?.errors) {
    const errorDetails = Object.values(err.response.data.errors).flat().join(", ");
    if (errorDetails) return errorDetails;
  }
  return err.response?.data?.message || err.response?.data?.detail || err.response?.data?.title || err.message || fallback;
};

const UserRegistration = () => {
    const navigate = useNavigate();
    const [isSubmitting, setIsSubmitting] = useState(false);

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm({
        defaultValues: {
            username: "",
            password: "",
            firstname: "",
            lastname: "",
        }
    });

    const onSubmit = async (data) => {
        if (!data.firstname.trim() || !data.lastname.trim() || !data.username.trim() || !data.password) {
            toaster.create({
                title: "Błąd walidacji",
                description: "Please fill out all of the fields",
                type: "error"
            });
            return;
        }

        setIsSubmitting(true);
        try {
            await registerUser({
                firstName: data.firstname.trim(),
                lastName: data.lastname.trim(),
                email: data.username.trim(),
                password: data.password
            });
            toaster.create({
                title: "Konto utworzone",
                description: "Account registered, proceed to login",
                type: "success"
            });
            navigate("/login");
        } catch (err) {
            console.error(err);
            toaster.create({
                title: "Błąd rejestracji",
                description: formatErrorMsg(err, "Account wasn't created, please verify data"),
                type: "error"
            });
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <VStack align={"center"} w="full">
            <Heading fontSize={"3xl"} fontWeight={"bold"} _dark={{ color: "white" }}>Create an account</Heading>
            <Text _dark={{ color: "whiteAlpha.700" }} pb={6}>Register as a car owner</Text>
            <form onSubmit={handleSubmit(onSubmit)} style={{ width: "100%" }}>
                <Stack gap="5" align="flex-start" w="full">
                    <Field.Root invalid={!!errors.firstname}>
                        <Field.Label>Name</Field.Label>
                        <InputGroup w="full" startElement={<Icon as={User} color={"gray.500"} boxSize={5} />}>
                            <Input 
                                {...register("firstname", { required: "Name is required" })} 
                                _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none" }} 
                                w="full" 
                                rounded="2xl" 
                                size="xl" 
                                placeholder="Jan" 
                                _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                            />
                        </InputGroup>
                        <Field.ErrorText>{errors.firstname?.message}</Field.ErrorText>
                    </Field.Root>

                    <Field.Root invalid={!!errors.lastname}>
                        <Field.Label>Last Name</Field.Label>
                        <InputGroup w="full" startElement={<Icon as={User} color={"gray.500"} boxSize={5} />}>
                            <Input 
                                {...register("lastname", { required: "Last name is required" })} 
                                _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none" }} 
                                w="full" 
                                rounded="2xl" 
                                size="xl" 
                                placeholder="Kowalski" 
                                _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                            />
                        </InputGroup>
                        <Field.ErrorText>{errors.lastname?.message}</Field.ErrorText>
                    </Field.Root>

                    <Field.Root invalid={!!errors.username}>
                        <Field.Label>e-Mail Address</Field.Label>
                        <InputGroup w="full" startElement={<Icon as={Mail} color={"gray.500"} boxSize={5} />}>
                            <Input 
                                type="email"
                                {...register("username", { required: "Email is required" })} 
                                _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none" }} 
                                w="full" 
                                rounded="2xl" 
                                size="xl" 
                                placeholder="name@example.com" 
                                _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                            />
                        </InputGroup>
                        <Field.ErrorText>{errors.username?.message}</Field.ErrorText>
                    </Field.Root>

                    <Field.Root invalid={!!errors.password}>
                        <Field.Label>Password</Field.Label>
                        <InputGroup w="full" startElement={<Icon as={Lock} color={"gray.500"} boxSize={5} />}>
                            <PasswordInput 
                                {...register("password", { required: "Password is required" })} 
                                _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none" }} 
                                w="full" 
                                rounded="2xl" 
                                size="xl" 
                                _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                                eyeColor={"brand.600"} 
                            />
                        </InputGroup>
                        <Field.ErrorText>{errors.password?.message}</Field.ErrorText>
                    </Field.Root>

                    <Button type="submit" loading={isSubmitting} rounded={"xl"} size={"lg"} h={"50px"} w={"full"} color="white" bg={"brand.600"} _hover={{ bg: "brand.700" }} mt={4}>
                        Register
                        <Icon as={ArrowRight} />
                    </Button>
                </Stack>
            </form>
            <HStack pt={4}>
                <Text fontSize={"sm"}>Already have an account?</Text>
                <Link asChild fontSize={"sm"} color={"brand.500"} _hover={{ color: "brand.600", textDecoration: "none" }}>
                    <RouterLink to="/login">Sign in</RouterLink>
                </Link>
            </HStack>
        </VStack>
    );
};

export default UserRegistration;