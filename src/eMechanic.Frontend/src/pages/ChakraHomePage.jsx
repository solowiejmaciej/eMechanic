import { Box, Container, Flex, HStack, Button, Image, Heading, SimpleGrid, VStack, Text, Icon, Center, Badge, InputGroup, Input, Separator } from "@chakra-ui/react"
import React, { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Quote, Search, Shield, Star, Car, Clock, MapPin, Plus, Wrench, Calendar, Filter } from "lucide-react";
import ChakraNavbar from "@/components/layout/ChakraNavbar";
import ChakraFooter from "@/components/layout/ChakraFooter";
import CustomLoader from "@/components/ui/CustomLoader";
import UserDashboard from "./UserDashboard";
import WorkshopDashboard from "./WorkshopDashboard";




const ChakraHomePage = () => {
    return (

        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
            <ChakraNavbar/>
            <WorkshopDashboard/>
            <ChakraFooter />
        </Box>




    )

}

export default ChakraHomePage;