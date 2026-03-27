using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SOUPIShared.Dtos;
using SOUPIShared.Models;


namespace SOUPIShared.Extensions
{
    public static class JobSequenceDtoExtensions
    {
        public static bool IsEquivalent(this JobSequenceDto jobSequenceDto, JobSequence jobSequence)
        {
            return jobSequenceDto.Id == jobSequence.Id &&
                jobSequenceDto.FirstJobId == jobSequence.FirstJobId &&
                jobSequenceDto.SecondJobId == jobSequence.SecondJobId; 
        }

        public static bool IsEquivalent(this JobSequenceDto firstJobSequenceDto, JobSequenceDto secondJobSequenceDto)
        {
            return firstJobSequenceDto.Id == secondJobSequenceDto.Id &&
                firstJobSequenceDto.FirstJobId == secondJobSequenceDto.FirstJobId &&
                firstJobSequenceDto.SecondJobId == secondJobSequenceDto.SecondJobId;
        }

        public static bool AreNonKeyPropertiesEquivalent(this JobSequenceDto jobSequenceDto, JobSequence jobSequence)
        {
            return jobSequenceDto.FirstJobId == jobSequence.FirstJobId &&
                jobSequenceDto.SecondJobId == jobSequence.SecondJobId;
        }

        public static bool AreNonKeyPropertiesEquivalent(this JobSequenceDto firstJobSequenceDto, JobSequenceDto secondJobSequenceDto)
        {
            return firstJobSequenceDto.FirstJobId == secondJobSequenceDto.FirstJobId &&
                firstJobSequenceDto.SecondJobId == secondJobSequenceDto.SecondJobId;
        }
    }
}
