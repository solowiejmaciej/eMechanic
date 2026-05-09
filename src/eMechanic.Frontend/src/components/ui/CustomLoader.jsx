import { Center, Icon, Text } from "@chakra-ui/react";
import { Wrench, Settings } from 'lucide-react';

const spinAnimationKeyframes = `
  @keyframes spin {
    from { transform: rotate(0deg); }
    to { transform: rotate(360deg); }
  }
`;

const CustomLoader = () => {
    return (
        <Center w="full" py={20} flexDirection="column" gap={6}>
            <style>{spinAnimationKeyframes}</style>

            <Center position="relative" w="80px" h="80px">

                <Icon
                    as={Settings}
                    h="55px"
                    w="55px"
                    color="brand.300"
                    _dark={{ color: "brand.700" }}
                    position="absolute"
                    css={{ animation: "spin 3s linear infinite" }}
                />

                <Icon
                    as={Wrench}
                    w="55px"
                    h="55px"
                    position="relative"
                    zIndex={1}
                    color="brand.600"
                    fill="gray.100"
                    _dark={{ color: "brand.500", fill: "#0F172A" }}
                    top="-3px"  
                    left="3px" 
                />

            </Center>
        </Center>
    );
};

export default CustomLoader;