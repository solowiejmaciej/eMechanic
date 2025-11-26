import React, { useState, useEffect, useRef } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Menu, X, ChevronDown, User, LogOut, Settings } from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import ThemeToggle from '../ui/ThemeToggle';
import logo from '../../assets/logo.png';
import { useAuth } from '../../context/AuthContext';

const Navbar = () => {
  const { user, logout } = useAuth();
  const [isScrolled, setIsScrolled] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isCollapsed, setIsCollapsed] = useState(false);
  const [lastScrollY, setLastScrollY] = useState(0);
  const [manuallyExpanded, setManuallyExpanded] = useState(false);
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const profileRef = useRef(null);
  const location = useLocation();

  useEffect(() => {
    const handleScroll = () => {
      const currentScrollY = window.scrollY;
      
      // Update scroll state
      setIsScrolled(currentScrollY > 20);
      
      // If manually expanded and user scrolls, collapse it
      if (manuallyExpanded && currentScrollY !== lastScrollY) {
        setIsCollapsed(true);
        setManuallyExpanded(false);
      }
      
      // Collapse/expand logic
      if (currentScrollY < 20) {
        // At top of page - always expand
        setIsCollapsed(false);
        setManuallyExpanded(false);
      } else if (currentScrollY > lastScrollY && currentScrollY > 100) {
        // Scrolling down - collapse
        setIsCollapsed(true);
        setManuallyExpanded(false);
      }
      
      setLastScrollY(currentScrollY);
    };

    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, [lastScrollY, manuallyExpanded]);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (profileRef.current && !profileRef.current.contains(event.target)) {
        setIsProfileOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const toggleNavbar = () => {
    setIsCollapsed(false);
    setManuallyExpanded(true);
  };

  return (
    <>
      <motion.nav
        animate={{
          height: isCollapsed ? '60px' : 'auto',
        }}
        transition={{ duration: 0.15, ease: [0.4, 0, 0.2, 1] }}
        className={`fixed top-0 left-0 right-0 z-[100] transition-all duration-150 ${
          isScrolled && location.pathname !== '/'
            ? 'bg-white/90 dark:bg-dark/90 backdrop-blur-xl shadow-lg border-b border-slate-200/50 dark:border-slate-800/50' 
            : 'bg-transparent'
        } ${isCollapsed ? 'py-2' : 'py-4'}`}
      >
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          {isCollapsed ? (
            // Collapsed state - button on the left
            <div className="flex justify-start items-center h-12">
              <button
                onClick={toggleNavbar}
                className="flex items-center gap-2 px-4 py-2 bg-primary/10 dark:bg-primary/20 hover:bg-primary/20 dark:hover:bg-primary/30 rounded-full transition-all group"
              >
                <img src={logo} alt="eMechanic" className="w-6 h-6 object-contain rounded-lg" />
                <span className="text-sm font-medium text-slate-700 dark:text-slate-200">Menu</span>
                <ChevronDown className="w-4 h-4 text-slate-600 dark:text-slate-300 group-hover:translate-y-0.5 transition-transform" />
              </button>
            </div>
          ) : (
            // Expanded state - full navbar
            <div className="flex justify-between items-center h-16">
              {/* Logo */}
              <Link to={user ? "/home" : "/"} className="flex items-center gap-3 group">
                <img src={logo} alt="eMechanic" className="w-14 h-14 object-contain rounded-2xl" />
                <span className="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 tracking-tight">
                  eMechanic
                </span>
              </Link>

              {/* Actions */}
              {/* Actions */}
              <div className="hidden md:flex items-center gap-3">
                <ThemeToggle />
                <div className="h-6 w-px bg-slate-200 dark:bg-slate-700 mx-1"></div>
                
                {user ? (
                  <div className="relative" ref={profileRef}>
                    <button
                      onClick={() => setIsProfileOpen(!isProfileOpen)}
                      className="flex items-center gap-2 pl-2 pr-3 py-1.5 rounded-full border border-slate-200 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-800 transition-all"
                    >
                      <div className="w-8 h-8 rounded-full bg-primary/10 dark:bg-primary/20 flex items-center justify-center text-primary font-bold text-sm">
                        {/* Use properties from /api/v1/users/me */}
                        {(user.firstName || user.email || '?')[0].toUpperCase()}
                        {(user.lastName || '')[0]?.toUpperCase()}
                      </div>
                      <span className="text-sm font-medium text-slate-700 dark:text-slate-200">
                        {user.firstName || user.email?.split('@')[0] || 'User'}
                      </span>
                      <ChevronDown className={`w-4 h-4 text-slate-400 transition-transform ${isProfileOpen ? 'rotate-180' : ''}`} />
                    </button>

                    <AnimatePresence>
                      {isProfileOpen && (
                        <motion.div
                          initial={{ opacity: 0, y: 10 }}
                          animate={{ opacity: 1, y: 0 }}
                          exit={{ opacity: 0, y: 10 }}
                          className="absolute right-0 mt-2 w-48 bg-white dark:bg-dark-card rounded-xl shadow-xl border border-slate-100 dark:border-slate-700 py-1 overflow-hidden"
                        >
                          <div className="px-4 py-3 border-b border-slate-100 dark:border-slate-800">
                            <p className="text-sm font-medium text-slate-900 dark:text-white truncate">
                              {user.firstName} {user.lastName}
                            </p>
                            <p className="text-xs text-slate-500 dark:text-slate-400 truncate">
                              {user.email}
                            </p>
                          </div>
                          <button
                            onClick={() => console.log('Profile clicked')}
                            className="w-full px-4 py-2 text-left text-sm text-slate-700 dark:text-slate-200 hover:bg-slate-50 dark:hover:bg-slate-800 flex items-center gap-2"
                          >
                            <User className="w-4 h-4" />
                            Profile
                          </button>
                          <button
                            onClick={() => console.log('Settings clicked')}
                            className="w-full px-4 py-2 text-left text-sm text-slate-700 dark:text-slate-200 hover:bg-slate-50 dark:hover:bg-slate-800 flex items-center gap-2"
                          >
                            <Settings className="w-4 h-4" />
                            Settings
                          </button>
                          <div className="border-t border-slate-100 dark:border-slate-800 my-1"></div>
                          <button
                            onClick={logout}
                            className="w-full px-4 py-2 text-left text-sm text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20 flex items-center gap-2"
                          >
                            <LogOut className="w-4 h-4" />
                            Log Out
                          </button>
                        </motion.div>
                      )}
                    </AnimatePresence>
                  </div>
                ) : (
                  <>
                    <Link
                      to="/login"
                      className="px-5 py-2.5 text-sm font-medium text-slate-700 dark:text-slate-200 hover:text-primary dark:hover:text-primary transition-colors"
                    >
                      Log In
                    </Link>
                    <Link
                      to="/register"
                      className="px-5 py-2.5 text-sm font-medium bg-primary hover:bg-primary-hover text-white rounded-full transition-all shadow-lg shadow-primary/25 hover:shadow-primary/40 hover:-translate-y-0.5"
                    >
                      Get Started
                    </Link>
                  </>
                )}
              </div>

              {/* Mobile Menu Button */}
              <div className="md:hidden flex items-center gap-4">
                <ThemeToggle />
                <button
                  className="p-2 text-slate-600 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg transition-colors"
                  onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                >
                  {isMobileMenuOpen ? <X /> : <Menu />}
                </button>
              </div>
            </div>
          )}
        </div>
      </motion.nav>

      {/* Mobile Menu Overlay */}
      <AnimatePresence>
        {isMobileMenuOpen && (
          <>
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              onClick={() => setIsMobileMenuOpen(false)}
              className="fixed inset-0 bg-black/20 dark:bg-black/50 backdrop-blur-sm z-40 md:hidden"
            />
            <motion.div
              initial={{ x: '100%' }}
              animate={{ x: 0 }}
              exit={{ x: '100%' }}
              transition={{ type: 'spring', damping: 25, stiffness: 200 }}
              className="fixed top-0 right-0 bottom-0 w-[280px] bg-white dark:bg-dark-card border-l border-slate-200 dark:border-slate-700 z-50 md:hidden shadow-2xl"
            >
              <div className="flex flex-col h-full p-6">
                <div className="flex justify-end mb-8">
                  <button
                    onClick={() => setIsMobileMenuOpen(false)}
                    className="p-2 text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-full transition-colors"
                  >
                    <X className="w-6 h-6" />
                  </button>
                </div>

                <div className="mt-auto pt-8 border-t border-slate-100 dark:border-slate-800 flex flex-col gap-3">
                  {user ? (
                    <>
                      <div className="flex items-center gap-3 px-4 py-3 bg-slate-50 dark:bg-slate-800/50 rounded-xl mb-2">
                        <div className="w-10 h-10 rounded-full bg-primary/10 dark:bg-primary/20 flex items-center justify-center text-primary font-bold">
                          {(user.firstName || user.email || '?')[0].toUpperCase()}
                          {(user.lastName || '')[0]?.toUpperCase()}
                        </div>
                        <div className="flex-1 min-w-0">
                          <p className="text-sm font-medium text-slate-900 dark:text-white truncate">
                            {user.firstName} {user.lastName}
                          </p>
                          <p className="text-xs text-slate-500 dark:text-slate-400 truncate">
                            {user.email}
                          </p>
                        </div>
                      </div>
                      <button
                        onClick={() => {
                          console.log('Profile clicked');
                          setIsMobileMenuOpen(false);
                        }}
                        className="w-full py-3 px-4 text-left text-slate-700 dark:text-slate-200 font-medium border border-slate-200 dark:border-slate-700 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors flex items-center gap-2"
                      >
                        <User className="w-5 h-5" />
                        Profile
                      </button>
                      <button
                        onClick={() => {
                          logout();
                          setIsMobileMenuOpen(false);
                        }}
                        className="w-full py-3 px-4 text-left text-red-600 font-medium border border-red-100 dark:border-red-900/30 rounded-xl hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors flex items-center gap-2"
                      >
                        <LogOut className="w-5 h-5" />
                        Log Out
                      </button>
                    </>
                  ) : (
                    <>
                      <Link
                        to="/login"
                        onClick={() => setIsMobileMenuOpen(false)}
                        className="w-full py-3 px-4 text-center text-slate-700 dark:text-slate-200 font-medium border border-slate-200 dark:border-slate-700 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors"
                      >
                        Log In
                      </Link>
                      <Link
                        to="/register"
                        onClick={() => setIsMobileMenuOpen(false)}
                        className="w-full py-3 px-4 text-center bg-primary text-white font-medium rounded-xl shadow-lg shadow-primary/25 hover:bg-primary-hover transition-colors"
                      >
                        Get Started
                      </Link>
                    </>
                  )}
                </div>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </>
  );
};

export default Navbar;
