import React from "react";
import { useForm } from "react-hook-form";


import { VStack, Heading, Text, Stack, Input, Button, HStack, Separator, Link, Icon, Field, InputGroup, SimpleGrid } from "@chakra-ui/react";
import { PasswordInput } from "@/components/ui/password-input";
import { Mail, Lock, ArrowRight, Building2, Phone, MapPin, MapPinned } from "lucide-react";

const WorkshopRegistration = () => {


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

    const onSubmit = (data) => console.log(data);
    return (
        <VStack align={"center"} >

            <Heading fontSize={"3xl"} fontWeight={"bold"}>Partner with eMechanic</Heading>
            <Text _dark={{ color: "whiteAlpha.700" }} pb={6}>Register your Workshop</Text>
            <form onSubmit={handleSubmit(onSubmit)}>
                <Heading>Account Information</Heading>
                <Separator borderColor="gray.300" _dark={{ borderColor: "whiteAlpha.200" }} />
                <HStack gap={5} py={4}>
                    <Field.Root invalid={!!errors.username}>
                        <Field.Label>Email Address</Field.Label>
                        <InputGroup startElement={<Icon as={Mail} color={"gray.500"} boxSize={5} />}>
                            <Input {...register("username")} _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: "orange.500" } }} w="full" rounded="2xl" size="xl" placeholder="name@example.com" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                        </InputGroup>
                        <Field.ErrorText>{errors.username?.message}</Field.ErrorText>
                    </Field.Root>
                    <Field.Root invalid={!!errors.password}>
                        <Field.Label>Password</Field.Label>
                        <InputGroup startElement={<Icon as={Lock} color={"gray.500"} boxSize={5} />}>
                            <PasswordInput {...register("password")} _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: "orange.500" } }} w="full" rounded="2xl" size="xl" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} eyeColor={"orange.500"} />
                        </InputGroup>
                        <Field.ErrorText>{errors.password?.message}</Field.ErrorText>
                    </Field.Root>
                </HStack>
                <Heading pt={4}>Workshop Details</Heading>
                <Separator borderColor="gray.300" _dark={{ borderColor: "whiteAlpha.200" }} />
                <SimpleGrid columns={2} gap="5" minW={"300px"} py={4}>
                    <Field.Root invalid={!!errors.workshopname}>
                        <Field.Label>Workshop Name</Field.Label>
                        <InputGroup startElement={<Icon as={Building2} color={"gray.500"} boxSize={5} />}>
                            <Input {...register("workshopname")} _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: "orange.500" } }} w="full" rounded="2xl" size="xl" placeholder="Legal Name" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                        </InputGroup>
                        <Field.ErrorText>{errors.workshopname?.message}</Field.ErrorText>
                    </Field.Root>
                    <Field.Root invalid={!!errors.displayname}>
                        <Field.Label>Display Name</Field.Label>
                        <InputGroup startElement={<Icon as={Building2} color={"gray.500"} boxSize={5} />}>
                            <Input {...register("displayname")} _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: "orange.500" } }} w="full" rounded="2xl" size="xl" placeholder="Public Name" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                        </InputGroup>
                        <Field.ErrorText>{errors.displayname?.message}</Field.ErrorText>
                    </Field.Root>
                    <Field.Root invalid={!!errors.contactemail}>
                        <Field.Label>Contact Email</Field.Label>
                        <InputGroup startElement={<Icon as={Mail} color={"gray.500"} boxSize={5} />}>
                            <Input {...register("contactemail")} _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: "orange.500" } }} w="full" rounded="2xl" size="xl" placeholder="contact@workshop.com" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                        </InputGroup>
                        <Field.ErrorText>{errors.contactemail?.message}</Field.ErrorText>
                    </Field.Root>
                    <Field.Root invalid={!!errors.phonenumber}>
                        <Field.Label>Phone Number</Field.Label>
                        <InputGroup startElement={<Icon as={Phone} color={"gray.500"} boxSize={5} />}>
                            <Input {...register("phonenumber")} _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: "orange.500" } }} w="full" rounded="2xl" size="xl" placeholder="+1 234 567 890" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                        </InputGroup>
                        <Field.ErrorText>{errors.phonenumber?.message}</Field.ErrorText>
                    </Field.Root>
                </SimpleGrid>
                <Field.Root invalid={!!errors.address}>
                    <Field.Label></Field.Label>
                    <InputGroup startElement={<Icon as={MapPin} color={"gray.500"} boxSize={5} />}>
                        <Input {...register("address")} _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: "orange.500" } }} w="full" rounded="2xl" size="xl" placeholder="123 Main St" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                    </InputGroup>
                    <Field.ErrorText>{errors.address?.message}</Field.ErrorText>
                </Field.Root>
                <SimpleGrid columns={3} gap={4} pt={4}>
                    <Field.Root invalid={!!errors.city}>
                        <Field.Label>City </Field.Label>
                       
                            <Input {...register("city")} _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: "orange.500" } }} w="full" rounded="2xl" size="xl" placeholder="New York" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                   
                        <Field.ErrorText>{errors.city?.message}</Field.ErrorText>
                    </Field.Root>
                    <Field.Root invalid={!!errors.postalcode}>
                        <Field.Label>Postal Code</Field.Label>
                       
                            <Input {...register("postalcode")} _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: "orange.500" } }} w="full" rounded="2xl" size="xl" placeholder="11-111" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                       
                        <Field.ErrorText>{errors.postalcode?.message}</Field.ErrorText>
                    </Field.Root>
                    <Field.Root invalid={!!errors.country}>
                        <Field.Label>Country</Field.Label>
                
                            <Input {...register("country")} _focus={{ borderColor: "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: "orange.500" } }} w="full" rounded="2xl" size="xl" placeholder="USA" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
        
                        <Field.ErrorText>{errors.country?.message}</Field.ErrorText>
                    </Field.Root>
                </SimpleGrid>


                <Button type="submit" rounded={"xl"} size={"lg"} h={"50px"} w={"full"} color="white" bg={"orange.500"} _hover={{ bg: "orange.600" }} mt={10}>
                    Create account
                    <Icon as={ArrowRight} />
                </Button>



            </form>
            <HStack pt={4}>
                <Text fontSize={"sm"}>Already have an account?</Text>
                <Link fontSize={"sm"} fontStyle={"none"} textDecoration={"none"} variant={"plain"} color={"orange.500"} _hover={{ color: "orange.600" }}>Sign in</Link>
            </HStack>

        </VStack>
    )


}

export default WorkshopRegistration;