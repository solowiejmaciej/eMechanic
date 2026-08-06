import React, { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate, Link as RouterLink } from "react-router-dom";
import { VStack, Heading, Text, Stack, Input, Button, HStack, Separator, Link, Icon, Field, InputGroup, SimpleGrid } from "@chakra-ui/react";
import { PasswordInput } from "@/components/ui/password-input";
import { Mail, Lock, ArrowRight, Building2, Phone, MapPin } from "lucide-react";
import { registerWorkshop } from "../../api/auth";
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

const WorkshopRegistration = () => {
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
            workshopname: "",
            displayname: "",
            contactemail: "",
            phonenumber: "",
            address: "",
            city: "",
            postalcode: "",
            country: "",
        }
    });

    const onSubmit = async (data) => {
        setIsSubmitting(true);
        try {
            await registerWorkshop({
                email: data.username.trim(),
                password: data.password,
                contactEmail: data.contactemail.trim(),
                name: data.workshopname.trim(),
                displayName: data.displayname.trim(),
                phoneNumber: data.phonenumber.trim(),
                address: data.address.trim(),
                city: data.city.trim(),
                postalCode: data.postalcode.trim(),
                country: data.country.trim()
            });

            toaster.create({
                title: "Konto warsztatu utworzone",
                description: "Rejestracja warsztatu zakończona sukcesem. Możesz się teraz zalogować.",
                type: "success"
            });
            navigate("/login");
        } catch (err) {
            console.error(err);
            toaster.create({
                title: "Błąd rejestracji",
                description: formatErrorMsg(err, "Nie udało się zarejestrować warsztatu. Sprawdź poprawność danych."),
                type: "error"
            });
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <VStack align={"center"} w="full">
            <Heading fontSize={"3xl"} fontWeight={"bold"} _dark={{ color: "white" }}>e-Mechanic partnership</Heading>
            <Text _dark={{ color: "whiteAlpha.700" }} pb={6}>Regsiter your workshop</Text>
            
            <form onSubmit={handleSubmit(onSubmit)} style={{ width: "100%" }}>
                <VStack align="stretch" gap={4} w="full">
                    <Heading size="md" _dark={{ color: "white" }}>Login data</Heading>
                    <Separator borderColor="gray.300" _dark={{ borderColor: "whiteAlpha.200" }} />
                    
                    <SimpleGrid columns={{ base: 1, md: 2 }} gap={4}>
                        <Field.Root invalid={!!errors.username}>
                            <Field.Label>E-mail address</Field.Label>
                            <InputGroup w="full" startElement={<Icon as={Mail} color={"gray.500"} boxSize={5} />}>
                                <Input 
                                    type="email"
                                    {...register("username", { required: "E-mail address is reuquired" })} 
                                    _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none" }} 
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
                                    _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none" }} 
                                    w="full" 
                                    rounded="2xl" 
                                    size="xl" 
                                    _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                                    eyeColor={"orange.500"} 
                                />
                            </InputGroup>
                            <Field.ErrorText>{errors.password?.message}</Field.ErrorText>
                        </Field.Root>
                    </SimpleGrid>

                    <Heading size="md" pt={4} _dark={{ color: "white" }}>Workshop data</Heading>
                    <Separator borderColor="gray.300" _dark={{ borderColor: "whiteAlpha.200" }} />
                    
                    <SimpleGrid columns={{ base: 1, md: 2 }} gap={4}>
                        <Field.Root invalid={!!errors.workshopname}>
                            <Field.Label>Full name (Company name)</Field.Label>
                            <InputGroup w="full" startElement={<Icon as={Building2} color={"gray.500"} boxSize={5} />}>
                                <Input 
                                    {...register("workshopname", { required: "Workshop name is required" })} 
                                    _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none" }} 
                                    w="full" 
                                    rounded="2xl" 
                                    size="xl" 
                                    placeholder="np. Auto-Fix Sp. z o.o." 
                                    _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                                />
                            </InputGroup>
                            <Field.ErrorText>{errors.workshopname?.message}</Field.ErrorText>
                        </Field.Root>
                        
                        <Field.Root invalid={!!errors.displayname}>
                            <Field.Label>Public name</Field.Label>
                            <InputGroup w="full" startElement={<Icon as={Building2} color={"gray.500"} boxSize={5} />}>
                                <Input 
                                    {...register("displayname", { required: "Display name is reuquired" })} 
                                    _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none" }} 
                                    w="full" 
                                    rounded="2xl" 
                                    size="xl" 
                                    placeholder="np. Auto-Fix Szybki Serwis" 
                                    _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                                />
                            </InputGroup>
                            <Field.ErrorText>{errors.displayname?.message}</Field.ErrorText>
                        </Field.Root>

                        <Field.Root invalid={!!errors.contactemail}>
                            <Field.Label>Contact e-mail</Field.Label>
                            <InputGroup w="full" startElement={<Icon as={Mail} color={"gray.500"} boxSize={5} />}>
                                <Input 
                                    type="email"
                                    {...register("contactemail", { required: "E-mail kontaktowy jest wymagany" })} 
                                    _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none" }} 
                                    w="full" 
                                    rounded="2xl" 
                                    size="xl" 
                                    placeholder="contact@autofix.pl" 
                                    _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                                />
                            </InputGroup>
                            <Field.ErrorText>{errors.contactemail?.message}</Field.ErrorText>
                        </Field.Root>
                        
                        <Field.Root invalid={!!errors.phonenumber}>
                            <Field.Label>Phone number</Field.Label>
                            <InputGroup w="full" startElement={<Icon as={Phone} color={"gray.500"} boxSize={5} />}>
                                <Input 
                                    {...register("phonenumber", { required: "Phone number is reuquired" })} 
                                    _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none" }} 
                                    w="full" 
                                    rounded="2xl" 
                                    size="xl" 
                                    placeholder="+48 123 456 789" 
                                    _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                                />
                            </InputGroup>
                            <Field.ErrorText>{errors.phonenumber?.message}</Field.ErrorText>
                        </Field.Root>
                    </SimpleGrid>

                    <Field.Root invalid={!!errors.address}>
                        <Field.Label>Street and house number</Field.Label>
                        <InputGroup w="full" startElement={<Icon as={MapPin} color={"gray.500"} boxSize={5} />}>
                            <Input 
                                {...register("address", { required: "Address is required" })} 
                                _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none" }} 
                                w="full" 
                                rounded="2xl" 
                                size="xl" 
                                placeholder="ul. Warsztatowa 15" 
                                _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                            />
                        </InputGroup>
                        <Field.ErrorText>{errors.address?.message}</Field.ErrorText>
                    </Field.Root>

                    <SimpleGrid columns={{ base: 1, md: 3 }} gap={4}>
                        <Field.Root invalid={!!errors.city}>
                            <Field.Label>Miasto</Field.Label>
                            <Input 
                                {...register("city", { required: "City is required" })} 
                                _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none" }} 
                                w="full" 
                                rounded="2xl" 
                                size="xl" 
                                placeholder="Poznań" 
                                _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                            />
                            <Field.ErrorText>{errors.city?.message}</Field.ErrorText>
                        </Field.Root>

                        <Field.Root invalid={!!errors.postalcode}>
                            <Field.Label>Postal code</Field.Label>
                            <Input 
                                {...register("postalcode", { required: "Postal code is required" })} 
                                _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none" }} 
                                w="full" 
                                rounded="2xl" 
                                size="xl" 
                                placeholder="60-101" 
                                _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                            />
                            <Field.ErrorText>{errors.postalcode?.message}</Field.ErrorText>
                        </Field.Root>

                        <Field.Root invalid={!!errors.country}>
                            <Field.Label>Country</Field.Label>
                            <Input 
                                {...register("country", { required: "Country required" })} 
                                _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none" }} 
                                w="full" 
                                rounded="2xl" 
                                size="xl" 
                                placeholder="Poland" 
                                _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} 
                            />
                            <Field.ErrorText>{errors.country?.message}</Field.ErrorText>
                        </Field.Root>
                    </SimpleGrid>

                    <Button type="submit" loading={isSubmitting} rounded={"xl"} size={"lg"} h={"50px"} w={"full"} color="white" bg={"orange.500"} _hover={{ bg: "orange.600" }} mt={6}>
                        Register workshop
                        <Icon as={ArrowRight} />
                    </Button>
                </VStack>
            </form>
            
            <HStack pt={4}>
                <Text fontSize={"sm"}>Have an account?</Text>
                <Link asChild fontSize={"sm"} color={"orange.500"} _hover={{ color: "orange.600", textDecoration: "none"}}>
                    <RouterLink to="/login">Sign in</RouterLink>
                </Link>
            </HStack>
        </VStack>
    );
};

export default WorkshopRegistration;