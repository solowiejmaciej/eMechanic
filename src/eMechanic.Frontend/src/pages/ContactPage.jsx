import React from 'react';
import Navbar from '../components/layout/Navbar';
import Footer from '../components/layout/Footer';
import { Mail, Phone, MapPin } from 'lucide-react';

const ContactPage = () => {
  return (
    <div className="min-h-screen bg-white dark:bg-slate-900 flex flex-col transition-colors">
      <Navbar />
      <div className="flex-grow pt-32 pb-12 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        <h1 className="text-3xl font-bold text-slate-900 dark:text-white mb-8">Contact Us</h1>
        
        <div className="grid md:grid-cols-2 gap-12">
          <div>
            <p className="text-lg text-slate-600 dark:text-slate-300 mb-8">
              Have questions or need assistance? We're here to help! Reach out to our support team using the contact information below or fill out the form.
            </p>
            
            <div className="space-y-6">
              <div className="flex items-start gap-4">
                <div className="w-12 h-12 rounded-xl bg-primary/10 dark:bg-primary/20 flex items-center justify-center text-primary">
                  <Mail className="w-6 h-6" />
                </div>
                <div>
                  <h3 className="font-bold text-slate-900 dark:text-white mb-1">Email Us</h3>
                  <p className="text-slate-600 dark:text-slate-400">support@emechanic.com</p>
                  <p className="text-slate-600 dark:text-slate-400">partners@emechanic.com</p>
                </div>
              </div>
              
              <div className="flex items-start gap-4">
                <div className="w-12 h-12 rounded-xl bg-primary/10 dark:bg-primary/20 flex items-center justify-center text-primary">
                  <Phone className="w-6 h-6" />
                </div>
                <div>
                  <h3 className="font-bold text-slate-900 dark:text-white mb-1">Call Us</h3>
                  <p className="text-slate-600 dark:text-slate-400">+1 (555) 123-4567</p>
                  <p className="text-sm text-slate-500 dark:text-slate-500">Mon-Fri, 9am - 5pm EST</p>
                </div>
              </div>
              
              <div className="flex items-start gap-4">
                <div className="w-12 h-12 rounded-xl bg-primary/10 dark:bg-primary/20 flex items-center justify-center text-primary">
                  <MapPin className="w-6 h-6" />
                </div>
                <div>
                  <h3 className="font-bold text-slate-900 dark:text-white mb-1">Visit Us</h3>
                  <p className="text-slate-600 dark:text-slate-400">123 Auto Lane</p>
                  <p className="text-slate-600 dark:text-slate-400">Mechanic City, MC 12345</p>
                </div>
              </div>
            </div>
          </div>
          
          <div className="bg-slate-50 dark:bg-slate-800 p-8 rounded-2xl border border-slate-100 dark:border-slate-700">
            <h2 className="text-xl font-bold text-slate-900 dark:text-white mb-6">Send us a message</h2>
            <form className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Name</label>
                <input type="text" className="w-full px-4 py-2 rounded-xl border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-primary outline-none transition-all" placeholder="Your name" />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Email</label>
                <input type="email" className="w-full px-4 py-2 rounded-xl border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-primary outline-none transition-all" placeholder="your@email.com" />
              </div>
              <div>
                <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Message</label>
                <textarea rows="4" className="w-full px-4 py-2 rounded-xl border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-primary outline-none transition-all" placeholder="How can we help?"></textarea>
              </div>
              <button type="button" className="w-full py-3 bg-primary text-white font-bold rounded-xl hover:bg-primary-hover transition-colors">
                Send Message
              </button>
            </form>
          </div>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default ContactPage;
