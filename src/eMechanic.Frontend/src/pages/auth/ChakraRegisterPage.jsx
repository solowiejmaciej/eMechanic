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
import UserRegistration from "./ChakraRegisterUser"
import WorkshopRegistration from "./ChakraRegisterWorkshop"


const ChakraRegister = () => {

    const [activeTab, setActiveTab] = useState('user');



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
        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
            <ChakraNavbar />
            <Box flex="1" p={90} mx="auto" mt={10}>
                <VStack align={"center"} w="full" minW={"400px"} minH={"720px"} h="full" display="flex" flexDirection="column" border="sm" rounded="2xl" p="6" boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }}>
                    
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
                    {activeTab === 'user' ? (
                        <UserRegistration />
                    ) : (
                        <WorkshopRegistration />
                    )}

                </VStack>
            </Box>
        </Box>


    )

}

export default ChakraRegister;