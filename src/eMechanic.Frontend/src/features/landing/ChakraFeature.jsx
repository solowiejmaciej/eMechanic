import { Box, Container, Flex, HStack, Button, Image, Heading, SimpleGrid, VStack, Text, Icon, Center, Badge } from "@chakra-ui/react"
import React, { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Quote, Search, Shield, Star, Car, Clock, MapPin, DollarSign, BarChart, ShieldCheck, BarChart3, Circle, Wrench } from "lucide-react";
import { transform } from "framer-motion";
import { motion } from "framer-motion";

const features = [
    {
        title: "Find Trusted Mechanics",
        descirption: "Browse verified workshops with real reviews and ratings from other car owners.",
        icon: Search
    },
    {
        title: "Transparent Pricing",
        descirption: "Get detailed quotes upfront. No hidden fees or surprise charges.",
        icon: DollarSign
    },
    {
        title: "Real-time Updates",
        descirption: "Track your repair status live and get notified when your car is ready.",
        icon: Clock
    },
    {
        title: "Business Growth",
        descirption: "Access a steady stream of new customers and grow your revenue",
        icon: BarChart3
    },
    {
        title: "Digital Management",
        descirption: "Manage bookings, quotes and customer communications in one dashboard",
        icon: ShieldCheck
    },
    {
        title: "Guaranteed Payments",
        descirption: "Secure payment processsing and automated invoicing for every job",
        icon: DollarSign
    }


]

const ChakraFeature = () => {
    return (
        <Box pb={100}>
            <VStack pt={200}>
                <Heading size="4xl" color="gray.900" _dark={{ color: "white" }} lineHeight="1rem">Everything you need to</Heading>
                <Heading size="4xl" color="brand.600" _dark={{ color: "brand.600" }}>manage vechicle repairs</Heading>
                <Text size="m" color="gray.60" fontWeight="semibold" _dark={{ color: "blue.200" }}>Whether you own a car or run a workshop, eMechanic streamlines the entire process.</Text>
            </VStack>
            <SimpleGrid columns={{ base: 1, md: 2 }} gap={16} pt={100} px={4} maxW="7xl" mx="auto">
                <VStack align="stretch">
                    <Flex align="center" gap={3}>
                        <Box pr={2} pl={2} pt={1} pb={2} bg="blue.50" rounded="lg" _dark={{ bg: "blue.900" }}>
                            <Icon as={Car} color="blue.600" boxSize={6} _dark={{ color: "brand.200" }} />
                        </Box>
                        <Heading size="2xl" fontWeight="bold" _dark={{ color: "white" }}>For Car Owners</Heading>
                    </Flex>
                    <VStack gap={5}>
                        {features.slice(0, 3).map((features, index) => (
                            <Box w="full" h="full" minH="240px" display="flex" flexDirection="column" key={index} border="sm" rounded="2xl" p={8} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                                <Flex align="center" justify="center" w={12} h={12} bg="brand.50" rounded="2xl" mb={6} color="brand.600" _dark={{ bg: "whiteAlpha.200", color: "brand.300" }}>
                                    <Icon boxSize={6} strokeWidth={1.5} as={features.icon} />
                                </Flex>
                                <Heading size="2xl">{features.title}</Heading>
                                <Text color="gray.600" _dark={{ color: "gray.400" }}>{features.descirption}</Text>
                            </Box>
                        ))}
                    </VStack>
                </VStack>
                <VStack align="start" spacing={8}>
                    <Flex align="center" gap={3}>
                        <Box pr={2} pl={2} pt={1} pb={2} bg="orange.200" rounded="lg" _dark={{ bg: "orange.900" }}>
                            <Icon as={Wrench} color="orange.400" boxSize={6} _dark={{ color: "orange.400" }} />
                        </Box>
                        <Heading size="2xl" fontWeight="bold" _dark={{ color: "white" }}>For Workshops</Heading>
                    </Flex>
                    <VStack gap={5}>
                        {features.slice(3, 6).map((features, index) => (
                            <Box w="full" h="full" minH="240px" display="flex" flexDirection="column" key={index} border="sm" rounded="2xl" p={8} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                                <Flex align="center" justify="center" w={12} h={12} bg="brand.50" rounded="2xl" mb={6} color="brand.600" _dark={{ bg: "whiteAlpha.200", color: "brand.300" }}>
                                    <Icon boxSize={6} strokeWidth={1.5} as={features.icon} />
                                </Flex>
                                <Heading size="2xl">{features.title}</Heading>
                                <Text color="gray.600" _dark={{ color: "gray.400" }}>{features.descirption}</Text>
                            </Box>
                        ))}
                    </VStack>
                </VStack>
            </SimpleGrid>
        </Box>
    )
}
export default ChakraFeature;