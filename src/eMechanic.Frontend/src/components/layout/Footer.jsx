import { Link } from 'react-router-dom';
import { Facebook, Twitter, Instagram, Linkedin, Mail, Phone, MapPin } from 'lucide-react';
import logo from '../../assets/logo.png';

const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="bg-white dark:bg-dark-card border-t border-slate-200 dark:border-slate-800 pt-10 pb-6 transition-colors mt-auto">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8 mb-8">
          {/* Brand */}
          <div className="space-y-3">
            <Link to="/" className="flex items-center gap-2">
              <div className="w-8 h-8 bg-primary rounded-full flex items-center justify-center overflow-hidden">
                <img src={logo} alt="eMechanic" className="w-8 h-8 object-cover" />
              </div>
              <span className="text-lg font-bold bg-clip-text text-transparent bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-200">
                eMechanic
              </span>
            </Link>
            <p className="text-slate-500 dark:text-slate-400 text-xs leading-relaxed">
              Connecting car owners with the best workshops. Reliable repairs, transparent pricing, and hassle-free booking.
            </p>
            <div className="flex gap-3">
              <a href="#" className="w-8 h-8 rounded-full bg-slate-100 dark:bg-slate-800 flex items-center justify-center text-slate-500 dark:text-slate-400 hover:bg-primary hover:text-white transition-all">
                <Facebook className="w-4 h-4" />
              </a>
              <a href="#" className="w-8 h-8 rounded-full bg-slate-100 dark:bg-slate-800 flex items-center justify-center text-slate-500 dark:text-slate-400 hover:bg-primary hover:text-white transition-all">
                <Twitter className="w-4 h-4" />
              </a>
              <a href="#" className="w-8 h-8 rounded-full bg-slate-100 dark:bg-slate-800 flex items-center justify-center text-slate-500 dark:text-slate-400 hover:bg-primary hover:text-white transition-all">
                <Instagram className="w-4 h-4" />
              </a>
              <a href="#" className="w-8 h-8 rounded-full bg-slate-100 dark:bg-slate-800 flex items-center justify-center text-slate-500 dark:text-slate-400 hover:bg-primary hover:text-white transition-all">
                <Linkedin className="w-4 h-4" />
              </a>
            </div>
          </div>

          {/* Quick Links */}
          <div>
            <h3 className="font-bold text-slate-900 dark:text-white mb-4 text-sm">Quick Links</h3>
            <ul className="space-y-2">
              <li>
                <Link to="/find-workshop" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors text-xs">Find a Workshop</Link>
              </li>
              <li>
                <Link to="/for-mechanics" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors text-xs">For Mechanics</Link>
              </li>
              <li>
                <Link to="/how-it-works" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors text-xs">How It Works</Link>
              </li>
              <li>
                <Link to="/about" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors text-xs">About Us</Link>
              </li>
            </ul>
          </div>

          {/* Support */}
          <div>
            <h3 className="font-bold text-slate-900 dark:text-white mb-4 text-sm">Support</h3>
            <ul className="space-y-2">
              <li>
                <Link to="/help" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors text-xs">Help Center</Link>
              </li>
              <li>
                <Link to="/terms" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors text-xs">Terms of Service</Link>
              </li>
              <li>
                <Link to="/privacy" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors text-xs">Privacy Policy</Link>
              </li>
              <li>
                <Link to="/contact" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors text-xs">Contact Support</Link>
              </li>
            </ul>
          </div>

          {/* Contact Info */}
          <div>
            <h3 className="font-bold text-slate-900 dark:text-white mb-4 text-sm">Contact Us</h3>
            <ul className="space-y-3">
              <li className="flex items-start gap-2 text-xs text-slate-500 dark:text-slate-400">
                <MapPin className="w-4 h-4 text-primary flex-shrink-0" />
                <span>123 Auto Lane, Mechanic City, MC 12345</span>
              </li>
              <li className="flex items-center gap-2 text-xs text-slate-500 dark:text-slate-400">
                <Phone className="w-4 h-4 text-primary flex-shrink-0" />
                <span>+1 (555) 123-4567</span>
              </li>
              <li className="flex items-center gap-2 text-xs text-slate-500 dark:text-slate-400">
                <Mail className="w-4 h-4 text-primary flex-shrink-0" />
                <span>support@emechanic.com</span>
              </li>
            </ul>
          </div>
        </div>

        <div className="pt-6 border-t border-slate-100 dark:border-slate-800 flex flex-col md:flex-row justify-between items-center gap-4">
          <p className="text-slate-500 dark:text-slate-400 text-xs">
            © {currentYear} eMechanic. All rights reserved.
          </p>
          <div className="flex gap-4 text-xs">
            <Link to="/terms" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors">Terms</Link>
            <Link to="/privacy" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors">Privacy</Link>
            <Link to="/cookies" className="text-slate-500 dark:text-slate-400 hover:text-primary transition-colors">Cookies</Link>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
