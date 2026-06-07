import { ChakraProvider } from "@chakra-ui/react";
import { system } from "../../theme";
import { Toaster } from "./toaster";

export function Provider({ children }) {
  return (
    <ChakraProvider value={system}>
      {children}
      <Toaster />
    </ChakraProvider>
  );
}