﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace XamarinTimeSheet.Models
{
    class WorkAssignmentOperationModel
    {
        public string Operation { get; set; }
        public string AssignmentTitle { get; set; }   
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
    }
}
