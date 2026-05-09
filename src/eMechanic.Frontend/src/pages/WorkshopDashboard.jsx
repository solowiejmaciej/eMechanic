import { Box, Container, Flex, HStack, Button, Image, Heading, SimpleGrid, VStack, Text, Icon, Center, Badge, InputGroup, Input, Separator } from "@chakra-ui/react"
import React, { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Quote, Search, Shield, Star, Car, Clock, MapPin, Plus, Wrench, Calendar, Filter } from "lucide-react";
import ChakraNavbar from "@/components/layout/ChakraNavbar";
import ChakraFooter from "@/components/layout/ChakraFooter";
import CustomLoader from "@/components/ui/CustomLoader";




const WorkshopDashboard = () => {
    return (

        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
            <Box flex="1" w={"full"} maxW="1400px" mx="auto" p={20}>


                <SimpleGrid w={"full"} columns={"3"} py={10} gap={10}>
                    <Box h="full" display="flex" flexDirection="column" border="sm" rounded="2xl" p={4} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                        <Flex align="center" gap={3}>
                            <Flex w={14} h={14} bg="orange.100" rounded="lg" _dark={{ bg: "orange.600/30" }} align={"center"} justify={"center"}>
                                <Icon as={Wrench} color="orange.500" boxSize={6} _dark={{ color: "brand.200" }} />
                            </Flex>
                            <VStack align={"flex-start"} justify={"center"} gap={0}>
                                <Text fontSize="sm" _dark={{ color: "whiteAlpha.800" }}> Active repairs</Text>
                                <Text fontSize="3xl" fontWeight={"bold"} _dark={{ color: "white" }}>0</Text>
                            </VStack>

                        </Flex>

                    </Box>
                    <Box h="full" display="flex" flexDirection="column" border="sm" rounded="2xl" p={4} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                        <Flex align="center" gap={3}>
                            <Flex w={14} h={14} bg="blue.100" rounded="lg" _dark={{ bg: "blue.900" }} align={"center"} justify={"center"}>
                                <Icon as={Car} color="blue.600" boxSize={6} _dark={{ color: "brand.200" }} />
                            </Flex>
                            <VStack align={"flex-start"} justify={"center"} gap={0}>
                                <Text fontSize="sm" _dark={{ color: "whiteAlpha.800" }}>Vehicles in shop</Text>
                                <Text fontSize="3xl" fontWeight={"bold"} _dark={{ color: "white" }}>0</Text>
                            </VStack>

                        </Flex>

                    </Box>
                    <Box h="full" display="flex" flexDirection="column" border="sm" rounded="2xl" p={4} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                        <Flex align="center" gap={3}>
                            <Flex w={14} h={14} bg="green.100" rounded="lg" _dark={{ bg: "green.600/20" }} align={"center"} justify={"center"}>
                                <Icon as={Plus} color="green.500" boxSize={6} _dark={{ color: "brand.200" }} />
                            </Flex>
                            <VStack align={"flex-start"} justify={"center"} gap={0}>
                                <Text fontSize="sm" _dark={{ color: "whiteAlpha.800" }}>New repair orders</Text>
                                <Text fontSize="3xl" fontWeight={"bold"} _dark={{ color: "white" }}>0</Text>
                            </VStack>

                        </Flex>

                    </Box>
                </SimpleGrid>
            </Box>
        </Box>




    )

}

export default WorkshopDashboard;