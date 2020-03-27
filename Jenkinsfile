pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/core/sdk:3.1-buster' 
            args '-p 3000:3000 --network=mynet -v /root/.nuget:/root/.nuget'// Persist nuget cache
        }
    }
    stages {
        stage('Restore Packages') { 
            steps {
                sh 'dotnet restore Leopard.API'
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build'
            }
        }
        // stage('UnitTest') {
        //     steps {
        //         sh 'dotnet test Leopard.Domain.Test'
        //     }
        // }
        stage('ApiTest') {
            environment {
                MONGO_HOST = 'mongo_test'
                BLOB_STORE = '/var/blob-store'
                MONGO_PASS = credentials('MONGO_TEST_PASS')
                ApiTestBaseUrl = 'http://localhost:5000'
            }
            steps {
                // Prerequisite: Mongo test db already running.'
                sh 'mkdir /var/blob-store'

                sh 'dotnet run --project Leopard.API > server-output.txt &'
                sh 'dotnet run --project Leopard.DBInit'

                sh 'dotnet test Leopard.API.Test'

                sh 'chmod u+x check-server-output.sh'
                sh './check-server-output.sh'
            }
        }
    }
}