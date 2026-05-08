import React from "react";
import { useForm } from "react-hook-form";


import { VStack, Heading, Text, Stack, Input, Button, HStack, Separator, Link, Icon, Field, InputGroup} from "@chakra-ui/react";
import { PasswordInput } from "@/components/ui/password-input";
import { Mail, Lock, ArrowRight } from "lucide-react";

const UserRegistration = () => {


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

    const onSubmit = (data) => console.log(data);
    return (
        <VStack align={"center"} >

            <Heading fontSize={"3xl"} fontWeight={"bold"}>Create Account</Heading>
            <Text _dark={{ color: "whiteAlpha.700" }} pb={6}>Join as a Car Owner</Text>
            <form onSubmit={handleSubmit(onSubmit)}>
                <Stack gap="10" align="flex-start" minW={"300px"}>
                    <Field.Root invalid={!!errors.firstname}>
                        <Field.Label>First Name</Field.Label>
                        <InputGroup startElement={<Icon as={Mail} color={"gray.500"} boxSize={5} />}>
                            <Input {...register("firstname")} _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none", _dark: { borderColor: "brand.600" } }} w="full" rounded="2xl" size="xl" placeholder="John" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                        </InputGroup>
                        <Field.ErrorText>{errors.firstname?.message}</Field.ErrorText>
                    </Field.Root>
                    <Field.Root invalid={!!errors.lastname}>
                        <Field.Label>Last Name</Field.Label>
                        <InputGroup startElement={<Icon as={Mail} color={"gray.500"} boxSize={5} />}>
                            <Input {...register("lastname")} _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none", _dark: { borderColor: "brand.600" } }} w="full" rounded="2xl" size="xl" placeholder="Doe" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                        </InputGroup>
                        <Field.ErrorText>{errors.lastname?.message}</Field.ErrorText>
                    </Field.Root>
                    <Field.Root invalid={!!errors.username}>
                        <Field.Label>Email Address</Field.Label>
                        <InputGroup startElement={<Icon as={Mail} color={"gray.500"} boxSize={5} />}>
                            <Input {...register("username")} _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none", _dark: { borderColor: "brand.600" } }} w="full" rounded="2xl" size="xl" placeholder="name@example.com" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                        </InputGroup>
                        <Field.ErrorText>{errors.username?.message}</Field.ErrorText>
                    </Field.Root>
                    <Field.Root invalid={!!errors.password}>
                        <Field.Label>Password</Field.Label>
                        <InputGroup startElement={<Icon as={Lock} color={"gray.500"} boxSize={5} />}>
                            <PasswordInput {...register("password")} _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none", _dark: { borderColor: "brand.600" } }} w="full" rounded="2xl" size="xl" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} eyeColor={"brand.600"} />
                        </InputGroup>
                        <Field.ErrorText>{errors.password?.message}</Field.ErrorText>
                    </Field.Root>

                    <Button type="submit" rounded={"xl"} size={"lg"} h={"50px"} w={"full"} color="white" bg={"brand.600"} _hover={{ bg: "brand.700" }}>
                        Create account
                        <Icon as={ArrowRight} />
                    </Button>


                </Stack>
            </form>
            <HStack pt={4}>
                <Text fontSize={"sm"}>Already have an account?</Text>
                <Link fontSize={"sm"} fontStyle={"none"} textDecoration={"none"} variant={"plain"} color={"brand.500"} _hover={{ color: "brand.600" }}>Sign in</Link>
            </HStack>

        </VStack>
    )


}

export default UserRegistration;