import { Box, Container, Flex, HStack, Button, Image, Heading, SimpleGrid, VStack, Text, Icon, Center, Badge, InputGroup, Input, Separator } from "@chakra-ui/react"
import React, { useState, useEffect } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { Quote, Search, Shield, Star, Car, Clock, MapPin, Plus, Wrench, Calendar, Filter } from "lucide-react";
import ChakraNavbar from "@/components/layout/ChakraNavbar";
import ChakraFooter from "@/components/layout/ChakraFooter";
import CustomLoader from "@/components/ui/CustomLoader";
import UserDashboard from "./UserDashboard";
import WorkshopDashboard from "./WorkshopDashboard";
import { useAuth } from "../context/AuthContext";

const ChakraHomePage = () => {
    const { user } = useAuth();
    const location = useLocation();
    const navigate = useNavigate();

    // Get initial tab from URL or default to garage/requests depending on role
    const getTabFromUrl = () => {
        const params = new URLSearchParams(location.search);
        const tab = params.get("tab");
        const validTabs = ["garage", "workshops", "requests", "repairs", "preferences", "profile", "documents", "reviews"];
        if (validTabs.includes(tab)) return tab;
        return user?.role === 'Workshop' ? "requests" : "garage";
    };

    const [activeMenu, setActiveMenu] = useState(getTabFromUrl());

    // Sync state if URL changes (e.g. back/forward navigation or top navbar profile click)
    useEffect(() => {
        const tab = getTabFromUrl();
        if (activeMenu !== tab) {
            setActiveMenu(tab);
        }
    }, [location.search]);

    // Sync URL if activeMenu changes (e.g. sidebar button click)
    useEffect(() => {
        const params = new URLSearchParams(location.search);
        if (params.get("tab") !== activeMenu) {
            params.set("tab", activeMenu);
            navigate(`?${params.toString()}`, { replace: true });
        }
    }, [activeMenu, navigate, location.search]);

    return (
        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
            <ChakraNavbar onProfileClick={() => setActiveMenu("profile")} />
            {user?.role === 'Workshop' ? (
                <WorkshopDashboard activeMenu={activeMenu} setActiveMenu={setActiveMenu} />
            ) : (
                <UserDashboard activeMenu={activeMenu} setActiveMenu={setActiveMenu} />
            )}
            <ChakraFooter />
        </Box>
    );
};

export default ChakraHomePage;