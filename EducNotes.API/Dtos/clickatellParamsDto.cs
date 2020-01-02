using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class clickatellParamsDto
    {
        public clickatellParamsDto()
        {
            Binary = false;
            validityPeriod = 1;
            Charset = "UTF-8";
        }
        
        public string Content { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public Boolean Binary { get; set; }
        public string ClientMsgId { get; set; }
        public DateTime scheduledDeliveryTime { get; set; }
        public string UserDataHeader { get; set; }
        public int validityPeriod { get; set; }
        public string Charset { get; set; }
    }
}