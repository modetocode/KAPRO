using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoetoOro
{
    class DanceSelector
    {
        static double averageLog10RamnotoLikelihood=-54.89;
        static double averageLog106n3nLikelihood=-95.97;
        static double averageLog10PajduskoLikelihood=-134.83;
        //first range (to where the likelihoods will be accepted)
        static double range = -40;
        //second range
        static double secondRange = -80;
        public static DanceType select(double log10RamnotoLikelihood, double log106n3nLikelihood, double log10PajduskoLikelihood)
        {
            double difRamnoto = log10RamnotoLikelihood - averageLog10RamnotoLikelihood;
            double dif6n3n = log106n3nLikelihood - averageLog106n3nLikelihood;
            double difPajdusko = log10PajduskoLikelihood - averageLog10PajduskoLikelihood;
            if (difRamnoto >= dif6n3n && difRamnoto >= difPajdusko)
            {
                //Dance Ramnoto is the most probable
                if (log10RamnotoLikelihood >= averageLog10RamnotoLikelihood + range)
                    return DanceType.Ramnoto;
                else
                    if (log10RamnotoLikelihood >= averageLog10RamnotoLikelihood + secondRange)
                        return DanceType.RamnotoLooking;
                    else
                        return DanceType.NoDance;
            }
            else if (dif6n3n >= difPajdusko && dif6n3n >= difRamnoto)
            {
                //Dance 6n3n is the most probable
                if (log106n3nLikelihood >= averageLog106n3nLikelihood + range)
                    return DanceType.O6n3n;
                else
                    if (log106n3nLikelihood >= averageLog106n3nLikelihood + secondRange)
                        return DanceType.O6n3nLooking;
                    else
                        return DanceType.NoDance;
            }
            else
            {
                //Dance Pajdusko is the most probable
                if (log10PajduskoLikelihood >= averageLog10PajduskoLikelihood + range)
                    return DanceType.Pajdusko;
                else
                    if (log10PajduskoLikelihood >= averageLog10PajduskoLikelihood + secondRange)
                        return DanceType.PajduskoLooking;
                    else
                        return DanceType.NoDance;
            }
        }
    }
}
