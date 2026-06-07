import { Provider } from "@/components/ui/provider"
import { Box, Stack, List, Button, Heading, InputGroup, SimpleGrid, Text, VStack, Flex, Icon, HStack, Collapsible, CollapsibleTrigger, CollapsibleContent, Field, Fieldset, For, Input, NativeSelect, Textarea, Link, Separator } from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import { LuSearch } from "react-icons/lu"
import { BarChart3, Calendar, CircleCheck, User, CircleCheckBigIcon, CircleQuestionMark, CloudLightning, FileText, Mail, MapPin, MessageCircle, Phone, Pin, Search, TicketX, Users, Wrench, Zap, Lock, MoveRight, ArrowRight } from "lucide-react"
import { BiQuestionMark } from "react-icons/bi"
import React, { useState } from "react"
import { PasswordInput } from "@/components/ui/password-input"
import { useForm } from "react-hook-form"
import { GoogleLogin } from "@react-oauth/google"
import { useAuth } from "../../context/AuthContext"
import { useNavigate, useLocation, Link as RouterLink } from "react-router-dom"
import { toaster } from "@/components/ui/toaster"

const ChakraLogin = () => {
    const [activeTab, setActiveTab] = useState('user');
    const [isLoading, setIsLoading] = useState(false);
    const { login, googleLogin } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm({
        defaultValues: {
            email: "",
            password: "",
        }
    });

    const onSubmit = async (data) => {
        setIsLoading(true);
        const result = await login(data.email, data.password, activeTab === 'workshop');
        setIsLoading(false);
        
        if (result.success) {
            toaster.create({
                title: "Zalogowano pomyślnie",
                description: "Zostałeś pomyślnie zalogowany.",
                type: "success"
            });
            const from = location.state?.from?.pathname || "/home";
            navigate(from, { replace: true });
        } else {
            toaster.create({
                title: "Błąd logowania",
                description: result.error || "Niepoprawne dane logowania.",
                type: "error"
            });
        }
    };

    const handleGoogleSuccess = async (credentialResponse) => {
        setIsLoading(true);
        const result = await googleLogin(credentialResponse.credential);
        setIsLoading(false);
        
        if (result.success) {
            toaster.create({
                title: "Zalogowano pomyślnie",
                description: "Zostałeś pomyślnie zalogowany przez Google.",
                type: "success"
            });
            const from = location.state?.from?.pathname || "/home";
            navigate(from, { replace: true });
        } else {
            toaster.create({
                title: "Błąd logowania",
                description: result.error || "Logowanie przez Google nie powiodło się.",
                type: "error"
            });
        }
    };

    return (
        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
            <ChakraNavbar />
            <Box flex="1" p={90} mx="auto" mt={10}>
                <VStack align={"center"} w="full" minW={"400px"} minH={"720px"} h="full" display="flex" flexDirection="column" border="sm" rounded="2xl" p="6" boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }}>
                    <Heading fontSize={"3xl"} fontWeight={"bold"}>Welcome Back</Heading>
                    <Text _dark={{ color: "whiteAlpha.700" }} pb={6}>Sign in to your account</Text>
                    <Flex p={1} bg="whiteAlpha.100" _dark={{ bg: "whiteAlpha.50" }} rounded="xl" mb={8} w="full">
                        <Button onClick={() => setActiveTab('user')} flex={1} variant="ghost" fontSize="sm" fontWeight="medium" rounded="lg" gap={2} h="10" bg={activeTab === 'user' ? "white" : "transparent"} color={activeTab === 'user' ? "brand.600" : "gray.600"} shadow={activeTab === 'user' ? "sm" : "none"} _dark={{ bg: activeTab === 'user' ? "whiteAlpha.50" : "transparent", color: activeTab === 'user' ? "brand.500" : "gray.400", }} _hover={{ color: activeTab !== 'user' ? "brand.500" : undefined, _dark: { bg: activeTab !== 'user' ? "gray.200" : undefined } }}>
                            <Icon as={User} boxSize={4} />
                            Car Owner
                        </Button>

                        <Button onClick={() => setActiveTab('workshop')} flex={1} variant="ghost" fontSize="sm" fontWeight="medium" rounded="lg" gap={2} h="10" bg={activeTab === 'workshop' ? "white" : "transparent"} color={activeTab === 'workshop' ? "orange.500" : "gray.600"} shadow={activeTab === 'workshop' ? "sm" : "none"} _dark={{ bg: activeTab === 'workshop' ? "whiteAlpha.50" : "transparent", color: activeTab === 'workshop' ? "orange.500" : "gray.400", }} _hover={{ color: activeTab !== 'workshop' ? "orange.400" : undefined, _dark: { bg: activeTab !== 'workshop' ? "gray.200" : undefined } }} >
                            <Icon as={Wrench} boxSize={4} />
                            Workshop
                        </Button>
                    </Flex>

                    <form onSubmit={handleSubmit(onSubmit)}>
                        <Stack gap="10" align="flex-start" minW={"300px"}>
                            <Field.Root invalid={!!errors.email}>
                                <Field.Label>Email Address</Field.Label>
                                <InputGroup startElement={<Icon as={Mail} color={"gray.500"} boxSize={5} />}>
                                    <Input {...register("email", { required: "Email jest wymagany" })} _focus={{ borderColor: activeTab === 'user' ? "brand.600" : "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: activeTab === 'user' ? "brand.600" : "orange.500" } }} w="full" rounded="2xl" size="xl" placeholder="name@example.com" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} />
                                </InputGroup>
                                <Field.ErrorText>{errors.email?.message}</Field.ErrorText>
                            </Field.Root>
                            <Field.Root invalid={!!errors.password}>
                                <Field.Label>Password</Field.Label>
                                <InputGroup startElement={<Icon as={Lock} color={"gray.500"} boxSize={5} />}>
                                    <PasswordInput {...register("password", { required: "Hasło jest wymagane" })} _focus={{ borderColor: activeTab === 'user' ? "brand.600" : "orange.500", borderWidth: "2px", outline: "none", _dark: { borderColor: activeTab === 'user' ? "brand.600" : "orange.500" } }} w="full" rounded="2xl" size="xl" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.300" }} eyeColor={activeTab === 'user' ? "brand.600" : "orange.500"} />
                                </InputGroup>
                                <Field.ErrorText>{errors.password?.message}</Field.ErrorText>
                            </Field.Root>
                            <Button loading={isLoading} type="submit" rounded={"xl"} size={"lg"} h={"50px"} w={"full"} color="white" bg={activeTab === 'user' ? "brand.600" : "orange.500"} _hover={{ bg: activeTab === 'user' ? "brand.700" : "orange.600" }}>
                                Sign In
                                <Icon as={ArrowRight} />
                            </Button>
                        </Stack>
                    </form>
                    <HStack w="full" py={8}>
                        <Separator flex={1} borderColor="gray.300" _dark={{ borderColor: "whiteAlpha.200" }} />
                        <Text fontSize="sm" color="gray.500" _dark={{ color: "gray.400" }} px={2} whiteSpace="nowrap">
                            Or continue with
                        </Text>
                        <Separator flex={1} borderColor="gray.300" _dark={{ borderColor: "whiteAlpha.200" }} />
                    </HStack>
                    <Box w="full" display="flex" justifyContent="center">
                        <GoogleLogin 
                            onSuccess={handleGoogleSuccess} 
                            onError={() => {
                                toaster.create({
                                    title: "Błąd logowania",
                                    description: "Logowanie przez Google nie powiodło się.",
                                    type: "error"
                                });
                            }} 
                            shape="pill" 
                        />
                    </Box>
                    <HStack pt={4}>
                        <Text fontSize={"sm"}>Don't have an account?</Text>
                        <Link as={RouterLink} to="/register" fontSize={"sm"} variant={"plain"} color={"brand.500"} _hover={{ color: "brand.600" }}>Sign up</Link>
                    </HStack>

                </VStack>
            </Box>
        </Box>
    )
}

export default ChakraLogin;