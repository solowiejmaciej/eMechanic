import React from 'react';
import { Search, Calendar, Wrench, CheckCircle } from 'lucide-react';
import Navbar from '../components/layout/Navbar';
import Footer from '../components/layout/Footer';

const Step = ({ number, icon: Icon, title, description }) => (
  <div className="flex flex-col items-center text-center max-w-sm mx-auto">
    <div className="w-16 h-16 bg-primary/10 rounded-full flex items-center justify-center mb-6 relative">
      <Icon className="w-8 h-8 text-primary" />
      <div className="absolute -top-2 -right-2 w-8 h-8 bg-secondary rounded-full flex items-center justify-center text-white font-bold shadow-lg">
        {number}
      </div>
    </div>
    <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-3">{title}</h3>
    <p className="text-slate-600 dark:text-slate-400 leading-relaxed">{description}</p>
  </div>
);

const HowItWorksPage = () => {
  return (
    <div className="min-h-screen bg-slate-50 dark:bg-dark-lighter transition-colors">
      <Navbar />
      
      <div className="pt-32 pb-20 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto">
        <div className="text-center mb-20">
          <h1 className="text-4xl lg:text-5xl font-bold text-slate-900 dark:text-white mb-6">
            How eMechanic Works
          </h1>
          <p className="text-xl text-slate-600 dark:text-slate-400 max-w-2xl mx-auto">
            Getting your car repaired shouldn't be complicated. We've simplified the process into 4 easy steps.
          </p>
        </div>

        <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-12 relative">
          {/* Connecting Line (Desktop) */}
          <div className="hidden lg:block absolute top-8 left-0 w-full h-0.5 bg-slate-200 dark:bg-slate-700 -z-10" />
          
          <Step
            number="1"
            icon={Search}
            title="Search"
            description="Enter your location and the service you need. Browse top-rated local workshops."
          />
          <Step
            number="2"
            icon={Calendar}
            title="Book"
            description="Compare quotes and availability. Book an appointment instantly online."
          />
          <Step
            number="3"
            icon={Wrench}
            title="Repair"
            description="Drop off your car. Track the repair progress in real-time through the app."
          />
          <Step
            number="4"
            icon={CheckCircle}
            title="Done"
            description="Pay securely online and pick up your car. Rate your experience."
          />
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default HowItWorksPage;
