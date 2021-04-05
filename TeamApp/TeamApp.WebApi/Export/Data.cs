using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamApp.WebApi.Export
{
    public static class Data
    {
        public static List<Student> Students = new List<Student>
        {
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Phan Cao Boi",
                Age=21,
            },
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Phan Cao Thap",
                Age=22,
            },
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Phan Cao Cao",
                Age=21,
            },
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Tran Thi Duc",
                Age=22,
            },
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Nguyen Van A",
                Age=21,
            },
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Le Thi Buoi",
                Age=21,
            },
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Tran Van Cam",
                Age=21,
            },
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Nguyen Tony",
                Age=22,
            },
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Doctor A",
                Age=20,
            },
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Ho Dinh An",
                Age=21,
            },
            new Student
            {
                Id=Guid.NewGuid().ToString(),
                Name="Tran A",
                Age=20,
            },
        };
    }
}
